using MediatR;
using SecretsSharingTool.Core.Handlers.Secret.Cleanse;

namespace SecretsSharingTool.Web.HostedServices;

public sealed class CleanseSecretsHostedService : BaseHostedService<CleanseSecretsHostedService>
{
    public CleanseSecretsHostedService(ILoggerFactory loggerFactory, IServiceProvider serviceProvider) : base(loggerFactory, serviceProvider)
    {
    }

    protected override string ServiceName => "Cleanse Secret Access Audit";
    protected override async void PerformTask(object? state)
    {
        var traceId = Guid.NewGuid().ToString();

        Logger.LogInformation(
            $"Executing secrets cleansing. {GetTimestamp()}. Trace ID: {traceId}");

        try
        {
            await using var scope = ServiceProvider.CreateAsyncScope();
            var mediatr = scope.ServiceProvider.GetService<IMediator>()!;
            await mediatr.Send(new CleanseSecretsCommand());
        }
        catch (Exception ex)
        {
            Logger.LogError(
                $"An error occurred while cleansing secrets. {GetTimestamp()}. TraceId: {traceId}",
                ex);
            return;
        }

        Logger.LogInformation(
            $"Completed secrets cleansing. {GetTimestamp()}. Trace ID: {traceId}");
    }
}