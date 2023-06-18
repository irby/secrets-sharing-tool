using CliWrap;
using SecretsSharingTool.Common.Tests.Database;
using Xunit.Abstractions;

namespace SecretsSharingTool.Common.Tests.Fixtures;

public abstract class BaseFixture
{
    protected readonly IMessageSink MessageSink;

    protected BaseFixture(IMessageSink messageSink)
    {
        MessageSink = messageSink;
    }
    
    protected async Task RunBatchFile(string command, string argument, string? workingDirectory, bool throwException = true)
    {
        OutputMessage($"Running: {command} {argument}");

        try
        {
            var cli = Cli.Wrap(command)
                .WithArguments(argument)
                .WithValidation(CommandResultValidation.None);

            if (workingDirectory is { })
            {
                cli.WithWorkingDirectory(workingDirectory);
            }

            await cli.ExecuteAsync();
        }
        catch(Exception ex)
        {
            if (throwException)
            {
                OutputMessage(ex.Message);
                throw;
            }
        }

        void OutputMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            MessageSink.OnMessage(new PrintableDiagnosticMessage(message));
        }
    }
}
