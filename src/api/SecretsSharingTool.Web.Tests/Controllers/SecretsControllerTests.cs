using System.Net;
using FluentAssertions;
using Newtonsoft.Json;
using SecretsSharingTool.Core.Handlers.Secret.Create;
using SecretsSharingTool.Core.Handlers.Secret.Retrieve;
using Xunit.Abstractions;

namespace SecretsSharingTool.Web.Tests.Controllers;

public class SecretsControllerTests : BaseControllerTests
{
    public SecretsControllerTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }
    
    [Fact]
    public async Task CreateSecret_WhenRequestIsValid_ReturnsCreatedWithResponse()
    {
        var request = new CreateSecretCommand()
        {
            Message = "Hello, World!",
            ExpireMinutes = 20,
        };
        var now = DateTimeOffset.Now;
        
        var response = await PostAsync("/api/secrets", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseString = await response.Content.ReadAsStringAsync();
        var responseBody = JsonConvert.DeserializeObject<CreateSecretCommandResponse>(responseString)!;

        responseBody.Key.Should().NotBeNull();
        responseBody.SecretId.Should().NotBe(Guid.Empty);
        responseBody.ExpireDateTime.Should().BeWithin(now.AddMinutes(21) - now.AddMinutes(20));
        responseBody.ExpireDateTimeEpoch.Should().BeGreaterOrEqualTo(now.AddMinutes(20).ToUnixTimeSeconds());
    }
    
    [Fact]
    public async Task CreateSecret_WhenRequestFails_ReturnsBadRequest()
    {
        var request = new CreateSecretCommand()
        {
            Message = "",
            ExpireMinutes = 20,
        };
        
        var response = await PostAsync("/api/secrets", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task RetrieveSecret_WhenRequestIsValid_ReturnsSecret()
    {
        var createRequest = new CreateSecretCommand()
        {
            Message = "Hello, World!",
            ExpireMinutes = 20,
        };
        
        var createResponse = await PostAsync("/api/secrets", createRequest);
        var createResponseString = await createResponse.Content.ReadAsStringAsync();
        var createResponseBody = JsonConvert.DeserializeObject<CreateSecretCommandResponse>(createResponseString)!;

        var retrieveResponse =
            await GetAsync($"/api/Secrets/{createResponseBody.SecretId}?key={createResponseBody.Key}");

        retrieveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrieveResponseString = await retrieveResponse.Content.ReadAsStringAsync();
        var retrieveResponseBody = JsonConvert.DeserializeObject<RetrieveSecretCommandResponse>(retrieveResponseString)!;

        retrieveResponseBody.Message.Should().Be("Hello, World!");
    }

    [Fact]
    public async Task RetrieveSecret_WhenRequestFails_ReturnsNotFound()
    {
        var retrieveResponse =
            await GetAsync($"/api/Secrets/{Guid.NewGuid()}?key=thiswillfail");

        retrieveResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}