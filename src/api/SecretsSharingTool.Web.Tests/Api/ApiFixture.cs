using SecretsSharingTool.Common.Tests.Exceptions;
using SecretsSharingTool.Common.Tests.Fixtures;
using Xunit.Abstractions;

namespace SecretsSharingTool.Web.Tests.Api;

public class ApiFixture : BaseFixture, IAsyncLifetime
{
    
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(TestConstants.ApiUrl) };
    private const int MaxNumberOfHealthChecks = 30;

    public ApiFixture(IMessageSink sink) : base(sink)
    {
    }
    
    public async Task InitializeAsync()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var projectDirectory = currentDirectory.Substring(0, currentDirectory.LastIndexOf("/src/", StringComparison.OrdinalIgnoreCase));
        var numAttempts = 0;

        await RunBatchFile("docker-compose", "--project-directory . -f env/local/docker-compose.local.yml up -d", projectDirectory, false);
        
        while (!await IsAppHealthy())
        {
            if (numAttempts >= MaxNumberOfHealthChecks)
            {
                throw new ServiceUnavailableException("Database fixture service could not be reached");
            }
            
            Thread.Sleep(1000);
            numAttempts++;
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<bool> IsAppHealthy()
    {
        try
        {
            var health = await _httpClient.GetAsync("/api/Health");
            return health.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
