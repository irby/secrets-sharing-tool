using MediatR;
using Microsoft.Extensions.Logging;
using SecretsSharingTool.Data.Database;

namespace SecretsSharingTool.Core.Handlers.Health;

public sealed class HealthCheckQueryHandler : IRequestHandler<HealthCheckQuery, HealthCheckQueryResponse>
{
    private readonly IDatabaseAccessor _databaseAccessor;
    private readonly ILogger<HealthCheckQueryHandler> _logger;
    
    public HealthCheckQueryHandler(IDatabaseAccessor databaseAccessor, ILogger<HealthCheckQueryHandler> logger)
    {
        _databaseAccessor = databaseAccessor;
        _logger = logger;
    }
    
    public async Task<HealthCheckQueryResponse> Handle(HealthCheckQuery query, CancellationToken cancellationToken)
    {
        var isSuccess = false;
        
        try
        {
            isSuccess = await _databaseAccessor.CanConnect();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return new HealthCheckQueryResponse()
        {
            IsSuccess = isSuccess
        };
    }
}
