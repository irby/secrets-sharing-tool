using MediatR;

namespace SecretsSharingTool.Core.Handlers.Secret.Create;

public class CreateSecretCommand : IRequest<CreateSecretCommandResponse>
{
    public string Message { get; set; }
    public long ExpireMinutes { get; set; }
}
