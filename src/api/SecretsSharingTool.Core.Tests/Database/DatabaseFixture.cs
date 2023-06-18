using CliWrap;
using CliWrap.Buffered;
using CustomEnvironmentConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SecretsSharingTool.Core.Configuration;
using SecretsSharingTool.Data;
using SecretsSharingTool.Common.Tests.Database;
using SecretsSharingTool.Common.Tests.Exceptions;
using SecretsSharingTool.Common.Tests.Fixtures;
using Xunit.Abstractions;

namespace SecretsSharingTool.Core.Tests.Database;

public class DatabaseFixture : BaseFixture, IAsyncLifetime
{
    private const int MaxNumberOfHealthChecks = 60;
    public readonly DatabaseConfiguration Configuration;

    public DatabaseFixture(IMessageSink sink) : base(sink)
    {
        Configuration = ConfigurationParser.ParsePosix<DatabaseConfiguration>("local.env", ConfigurationTypeEnum.PreferFile);
    }
    
    public async Task InitializeAsync()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var projectDirectory = currentDirectory.Substring(0, currentDirectory.LastIndexOf("/src/", StringComparison.OrdinalIgnoreCase));
        
        await RunBatchFile("docker-compose", "--project-directory . -f env/local/docker-compose.local.yml up -d secrets-db", projectDirectory);

        var healthChecks = 0;
        
        // Wait for DB to become available
        while (!await IsHealthy("secrets-db", projectDirectory))
        {
            if (healthChecks >= MaxNumberOfHealthChecks)
            {
                throw new ServiceUnavailableException("Database fixture service could not be reached");
            }
            healthChecks++;
            Thread.Sleep(100);
        }

        await RunDatabaseMigrations();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<bool> IsHealthy(string serviceName, string? workingDirectory)
    {
        var cli = Cli.Wrap("docker")
            .WithArguments("inspect --format '{{.State.Health.Status}}' " + serviceName)
            .WithValidation(CommandResultValidation.None);

        if (workingDirectory is { })
        {
            cli.WithWorkingDirectory(workingDirectory);
        }
        
        void OutputMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            MessageSink.OnMessage(new PrintableDiagnosticMessage(message));
        }

        var result = await cli.ExecuteBufferedAsync();
        var output = result.StandardOutput.Replace("\n", "").Replace("'", "").Trim();
        return output is "healthy" or "starting";
    }

    private async Task RunDatabaseMigrations()
    {
        var collection = new ServiceCollection();
        collection.AddDatabase(Configuration.BuildConnectionString(true));

        var db = collection.BuildServiceProvider().GetService<AppUnitOfWork>()!;
        await db.Database.MigrateAsync();
    }
}
