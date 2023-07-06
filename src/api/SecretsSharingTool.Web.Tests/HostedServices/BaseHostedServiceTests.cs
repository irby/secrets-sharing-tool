using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SecretsSharingTool.Web.HostedServices;

namespace SecretsSharingTool.Web.Tests.HostedServices;

public abstract class BaseHostedServiceTests<TService> where TService : BaseHostedService<TService>
{
    protected readonly Mock<IMediator> MockMediator = new ();
    private readonly IServiceProvider _serviceProvider;
    
    protected BaseHostedServiceTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        serviceCollection.AddSingleton(MockMediator.Object);
        serviceCollection.AddTransient<TService>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    protected TService GetService() => _serviceProvider.GetService<TService>()!;
}