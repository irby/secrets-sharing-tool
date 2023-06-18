using MediatR;
using SecretsSharingTool.Core.Handlers.Secret.Cleanse;

namespace SecretsSharingTool.Web.HostedServices;

public class CleanseSecretsHostedService : IHostedService, IDisposable
{
    private readonly ILogger<CleanseSecretsHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private Timer? _timer;

    public CleanseSecretsHostedService(ILogger<CleanseSecretsHostedService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cleanse secrets service running.");

        _timer = new Timer(CleanseSecrets, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
        
        return Task.CompletedTask;
    }

    private async void CleanseSecrets(object? state)
    {
        var traceId = Guid.NewGuid().ToString();
        
        _logger.LogInformation($"Executing secrets cleansing. {DateTimeOffset.UtcNow:MM/dd/yyyy HH:mm:ss.fff tt}. Trace ID: {traceId}");

        try
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var mediatr = scope.ServiceProvider.GetService<IMediator>()!;
            await mediatr.Send(new CleanseSecretsCommand());
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while cleansing secrets. {DateTimeOffset.UtcNow:MM/dd/yyyy HH:mm:ss.fff tt}. TraceId: {traceId}", ex);
            return;
        }
        
        _logger.LogInformation($"Completed secrets cleansing. {DateTimeOffset.UtcNow:MM/dd/yyyy HH:mm:ss.fff tt}. Trace ID: {traceId}");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cleanse secrets service is stopping.");
        
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}