using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SecretSharingTool.Data.Database;
using SecretsSharingTool.Core.Erase;

namespace SecretsSharingTool.Api.Queue
{
    public class SecretInvalidationBackgroundService : IBackgroundQueueService
    {
        private static readonly SecretInvalidationService? Thread = new SecretInvalidationService("Secret Invalidation Service");

        private readonly ILogger<SecretInvalidationService> _logger;
        private readonly EraseSecretMessageService _handler;

        public SecretInvalidationBackgroundService(ILogger<SecretInvalidationService> logger, EraseSecretMessageService handler)
        {
            _handler = handler;
            _logger = logger;
        }
        
        public async Task Start()
        {
            if (Thread != null)
            {
                CheckNotNull(_logger, "Logger");
                CheckNotNull(_handler, "Erase Secret Message Handler");
                
                Thread.Logger = _logger;
                Thread.Handler = _handler;

                Thread.Start();
            }
        }

        public void Stop() => Thread?.Stop();
        public void Process() => Thread?.Process();

        private void CheckNotNull(object variable, string description)
        {
            if (variable is null)
            {
                throw new ArgumentException($"Item cannot be null: {description}", nameof(variable));
            }
        }
    }
}