using MediatR;

namespace SecretsSharingTool.Core.Create
{
    public class SecretCreationCommand : IRequest<SecretCreationCommandHandlerResponse>
    {
        public string Message { get; set; }
        public int SecondsToLive { get; set; }
    }
}

