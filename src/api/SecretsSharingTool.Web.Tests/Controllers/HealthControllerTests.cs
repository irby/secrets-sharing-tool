using System.Net;
using FluentAssertions;
using Xunit.Abstractions;

namespace SecretsSharingTool.Web.Tests.Controllers;

public class HealthControllerTests : BaseControllerTests
{
    public HealthControllerTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }
    
    [Fact]
    public async Task HealthCheck_WhenCalled_ReturnsOk()
    {
        var healthResponse = await GetAsync("/api/Health");
        healthResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}