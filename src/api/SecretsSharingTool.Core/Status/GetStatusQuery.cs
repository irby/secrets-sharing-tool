using MediatR;

namespace SecretsSharingTool.Core.Status
{
    public sealed class GetStatusQuery : IRequest<bool>
    {
    }
}