using Microsoft.Extensions.DependencyInjection;
using Moq;
using SecretsSharingTool.Core.Handlers.Health;
using SecretsSharingTool.Core.Tests.Database;
using SecretsSharingtool.Data.Database;

namespace SecretsSharingTool.Core.Tests.Handlers.Health;

public class HealthCheckQueryHandlerTests : BaseHandlerTest
{
    private readonly Mock<IDatabaseAccessor> _mockDbAccessor = new ();
    
    public HealthCheckQueryHandlerTests(DatabaseFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Handle_WhenCanConnectIsSuccessful_ReturnsTrue()
    {
        _mockDbAccessor.Setup(p => p.CanConnect()).ReturnsAsync(true);

        var response = await Mediator.Send(new HealthCheckQuery());
        
        Assert.True(response.IsSuccess);
    }
    
    [Fact]
    public async Task Handle_WhenCanConnectIsNotSuccessful_ReturnsFalse()
    {
        _mockDbAccessor.Setup(p => p.CanConnect()).ReturnsAsync(false);

        var response = await Mediator.Send(new HealthCheckQuery());
        
        Assert.False(response.IsSuccess);
    }
    
    [Fact]
    public async Task Handle_WhenCanConnectThrowsException_ReturnsFalse()
    {
        _mockDbAccessor.Setup(p => p.CanConnect()).Throws(new Exception("Database is down!"));

        var response = await Mediator.Send(new HealthCheckQuery());
        
        Assert.False(response.IsSuccess);
    }

    protected override void ConfigureAdditionalServices()
    {
        ServiceCollection.AddTransient(_ => _mockDbAccessor.Object);
    }
}