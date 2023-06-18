using MediatR;

namespace SecretsSharingTool.Core.Handlers.Secret.Retrieve;

public class RetrieveSecretCommand : IRequest<RetrieveSecretCommandResponse>
{
    public Guid SecretId { get; set; }
    public string Key { get; set; }
}
