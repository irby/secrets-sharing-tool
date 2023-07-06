using MediatR;
using SecretsSharingTool.Core.Handlers.SecretAccessAudit.Cleanse;

namespace SecretsSharingTool.Web.HostedServices;

public class CleanseSecretAccessAuditsHostedService : BaseHostedService<CleanseSecretAccessAuditsHostedService>
{
    public CleanseSecretAccessAuditsHostedService(ILoggerFactory loggerFactory, IServiceProvider serviceProvider) : base(loggerFactory, serviceProvider)
    {
    }

    protected override string ServiceName => "Cleanse Secret Access Audit";

    protected override async void PerformTask(object? state)
    {
        var traceId = Guid.NewGuid().ToString();
        
        Logger.LogInformation($"Executing secrets audit cleansing. {GetTimestamp()}. Trace ID: {traceId}");

        try
        {
            await using var scope = ServiceProvider.CreateAsyncScope();
            var mediatr = scope.ServiceProvider.GetService<IMediator>()!;
            await mediatr.Send(new CleanseSecretAccessAuditsCommand());
        }
        catch (Exception ex)
        {
            Logger.LogError($"An error occurred while cleansing secrets audits. {GetTimestamp()}. TraceId: {traceId}", ex);
            return;
        }
        
        Logger.LogInformation($"Completed secrets audits cleansing. {GetTimestamp()}. Trace ID: {traceId}");
    }
}