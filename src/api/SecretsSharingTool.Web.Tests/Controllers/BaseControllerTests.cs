using System.Net.Http.Json;
using SecretsSharingTool.Web.Tests.Api;
using Xunit.Abstractions;

namespace SecretsSharingTool.Web.Tests.Controllers;

[Collection(ApiCollection.Name)]
public abstract class BaseControllerTests
{
    private readonly ITestOutputHelper _outputHelper;
    
    protected BaseControllerTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }
    
    private const string ApiBaseUrl = TestConstants.ApiUrl;
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(ApiBaseUrl) };

    protected async Task<HttpResponseMessage> GetAsync(string uri)
    {
        _outputHelper.WriteLine($"Sending GET request to {uri}");
        var response = await _httpClient.GetAsync(uri);
        await ParseResponse(uri, response);
        return response;
    }

    protected async Task<HttpResponseMessage> PostAsync(string uri, object value)
    {
        _outputHelper.WriteLine($"Sending POST request to {uri}");
        var response = await _httpClient.PostAsJsonAsync(uri, value);
        await ParseResponse(uri, response);
        return response;
    }

    private async Task ParseResponse(string uri, HttpResponseMessage response)
    {
        _outputHelper.WriteLine($"Response from {uri} returned: {response.StatusCode}");
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _outputHelper.WriteLine($"Error message received from {uri}: {content}");
        }
    }
}