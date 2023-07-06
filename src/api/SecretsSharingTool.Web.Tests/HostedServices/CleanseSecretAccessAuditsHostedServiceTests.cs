using Moq;
using SecretsSharingTool.Core.Handlers.SecretAccessAudit.Cleanse;
using SecretsSharingTool.Web.HostedServices;

namespace SecretsSharingTool.Web.Tests.HostedServices;

public class CleanseSecretAccessAuditsHostedServiceTests : BaseHostedServiceTests<CleanseSecretAccessAuditsHostedService>
{
    [Fact]
    public async Task StartAsync_WhenCalled_InvokesMediatrCall()
    {
        using var service = GetService();
        await service.StartAsync(new CancellationToken());
        MockMediator.Verify(p => p.Send(It.IsAny<CleanseSecretAccessAuditsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}