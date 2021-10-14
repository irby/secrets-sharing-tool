using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SecretSharingTool.Data.Database;
using SecretsSharingTool.Core.Erase;

namespace SecretsSharingTool.Api.Queue
{
    public class SecretInvalidationService : BackgroundService
    {
        public ILogger<SecretInvalidationService> Logger;
        public EraseSecretMessageService Handler;
        
        public SecretInvalidationService(string name) : base(name)
        {
            AutomaticWakeup = 15 * 60 * 1000; // 15 minutes
        }

        protected override async Task WorkerTask()
        {
            Logger.LogDebug("Starting erase expired secrets task");
            
            await Handler.Handle(CancellationToken.None);
        }

        protected override void Log(Exception ex)
        {
            Logger.LogError(ex, "An error has occurred");
        }

        protected override void Log(string message)
        {
            Logger.LogInformation(message);
        }
    }
}