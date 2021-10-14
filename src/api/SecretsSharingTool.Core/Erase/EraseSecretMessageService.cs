using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecretSharingTool.Data.Database;
using SecretsSharingTool.Core.Shared;

namespace SecretsSharingTool.Core.Erase
{
    public sealed class EraseSecretMessageService
    {
        private readonly ILogger<EraseSecretMessageService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public EraseSecretMessageService(IServiceScopeFactory serviceScopeFactory, ILogger<EraseSecretMessageService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }
        
        public async Task Handle(CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var appUnitOfWork = (AppUnitOfWork)scope.ServiceProvider.GetService(typeof(AppUnitOfWork));
                
                var invalidSecrets = await appUnitOfWork!.Secrets.Where(p => p.Message != null
                                                                             && (p.ExpireDateTime < DateTimeOffset.UtcNow || !p.IsActive))
                    .ToListAsync(cancellationToken);

                foreach (var secret in invalidSecrets)
                {
                    var status = secret.ExpireDateTime < DateTimeOffset.UtcNow ? "expired" : !secret.IsActive ? "inactive" : string.Empty;
                    _logger.LogInformation($"Erasing message for {status} secret {secret.Id}");
                    secret.Clear();
                }
            
                await appUnitOfWork.SaveChangesAsync(cancellationToken);
            }
            
        }
    }
}