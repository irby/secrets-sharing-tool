using MediatR;

namespace SecretsSharingTool.Core.Handlers.Health;

public sealed class HealthCheckQuery : IRequest<HealthCheckQueryResponse>
{
}