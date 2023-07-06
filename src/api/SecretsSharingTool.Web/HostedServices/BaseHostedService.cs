namespace SecretsSharingTool.Web.HostedServices;

public abstract class BaseHostedService<TService> : IHostedService, IDisposable where TService : class
{
    protected BaseHostedService(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
    {
        Logger = loggerFactory.CreateLogger<TService>();
        ServiceProvider = serviceProvider;
    }
    
    private Timer? _timer;
    
    protected readonly IServiceProvider ServiceProvider;
    protected readonly ILogger<TService> Logger;
    protected abstract string ServiceName { get; }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation($"{ServiceName} service running.");

        _timer = new Timer(PerformTask, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation($"{ServiceName} service is stopping.");
        
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    protected abstract void PerformTask(object? state);

    protected string GetTimestamp() => $"{DateTimeOffset.UtcNow:MM/dd/yyyy HH:mm:ss.fff tt}";
}