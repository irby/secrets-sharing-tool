using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecretsSharingTool.Core.Interfaces;
using SecretsSharingTool.Data;

namespace SecretsSharingTool.Core.Handlers.SecretAccessAudit.Cleanse;

public sealed class CleanseSecretAccessAuditsCommandHandler : BaseRequestHandler<CleanseSecretAccessAuditsCommand>
{
    public CleanseSecretAccessAuditsCommandHandler(AppUnitOfWork db, ILogger<CleanseSecretAccessAuditsCommandHandler> logger, IDateTimeProvider dateTimeProvider) : base(db, logger)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    private readonly IDateTimeProvider _dateTimeProvider;
    private const int BatchSize = 50;

    public override async Task<Unit> Handle(CleanseSecretAccessAuditsCommand command, CancellationToken cancellationToken)
    {
        var expiredAudits = AppUnitOfWork.AuditRecords.Where(p => p.CreatedOn.AddDays(180) <
                                                                  _dateTimeProvider.GetCurrentDateTimeOffset());

        var count = await expiredAudits.CountAsync(cancellationToken);
        var batches = (count / BatchSize) + 1;

        for (var i = 0; i < batches; i++)
        {
            var batch = await expiredAudits.Take(BatchSize).ToListAsync(cancellationToken);
            AppUnitOfWork.AuditRecords.RemoveRange(batch);
            await AppUnitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}