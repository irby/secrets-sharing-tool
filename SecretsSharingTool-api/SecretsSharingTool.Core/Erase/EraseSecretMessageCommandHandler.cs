using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecretSharingTool.Data.Database;
using SecretsSharingTool.Core.Shared;

namespace SecretsSharingTool.Core.Erase
{
    public sealed class EraseSecretMessageCommandHandler : BaseRequestHandler<EraseSecretMessageCommand>
    {
        public EraseSecretMessageCommandHandler(AppUnitOfWork appUnitOfWork, ILogger<EraseSecretMessageCommandHandler> logger) : base(appUnitOfWork, logger)
        {
        }
        
        public override async Task<Unit> Handle(EraseSecretMessageCommand request, CancellationToken cancellationToken)
        {
            var invalidSecrets = await AppUnitOfWork.Secrets.Where(p => p.Message != null
                                                                  && (p.ExpireDateTime < DateTime.UtcNow || !p.IsActive))
                                                                    .ToListAsync(cancellationToken);

            foreach (var secret in invalidSecrets)
            {
                var status = secret.ExpireDateTime < DateTime.UtcNow ? "expired" : !secret.IsActive ? "inactive" : string.Empty;
                Logger.LogInformation($"Erasing message for {status} secret {secret.Id}");
                secret.Message = null;
                secret.SetModifiedAndInactive();
            }
            
            await AppUnitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}