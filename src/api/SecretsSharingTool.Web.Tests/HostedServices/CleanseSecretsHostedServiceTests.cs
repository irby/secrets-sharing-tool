using Moq;
using SecretsSharingTool.Core.Handlers.Secret.Cleanse;
using SecretsSharingTool.Web.HostedServices;

namespace SecretsSharingTool.Web.Tests.HostedServices;

public class CleanseSecretsHostedServiceTests : BaseHostedServiceTests<CleanseSecretsHostedService>
{
    [Fact]
    public async Task StartAsync_WhenCalled_InvokesMediatrCall()
    {
        using var service = GetService();
        await service.StartAsync(new CancellationToken());
        MockMediator.Verify(p => p.Send(It.IsAny<CleanseSecretsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}