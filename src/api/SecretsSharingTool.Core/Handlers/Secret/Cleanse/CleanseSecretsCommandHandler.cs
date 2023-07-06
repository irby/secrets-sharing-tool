using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecretsSharingTool.Core.Interfaces;
using SecretsSharingTool.Data;

namespace SecretsSharingTool.Core.Handlers.Secret.Cleanse;

public class CleanseSecretsCommandHandler : BaseRequestHandler<CleanseSecretsCommand>
{
    public CleanseSecretsCommandHandler(AppUnitOfWork appUnitOfWork, ILogger<CleanseSecretsCommandHandler> logger, IDateTimeProvider dateTimeProvider) : base(appUnitOfWork, logger)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    private readonly IDateTimeProvider _dateTimeProvider;
    private const int BatchSize = 50;

    public override async Task<Unit> Handle(CleanseSecretsCommand request, CancellationToken cancellationToken)
    {
        var expiredSecrets = AppUnitOfWork.Secrets.Where(p => p.EncryptedMessage != null
                                                                  && (!p.IsActive ||
                                                                      p.CreatedOn.AddMinutes(p.ExpiryMinutes) <
                                                                      _dateTimeProvider.GetCurrentDateTimeOffset()));

        var count = await expiredSecrets.CountAsync(cancellationToken);
        var batches = (count / BatchSize) + 1;

        for (var i = 0; i < batches; i++)
        {
            var batch = await expiredSecrets.Take(BatchSize).ToListAsync(cancellationToken);
            batch.ForEach(p => p.Deactivate());
            await AppUnitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
