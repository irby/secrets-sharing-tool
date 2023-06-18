using Xunit.Sdk;

namespace SecretsSharingTool.Common.Tests.Database;

public class PrintableDiagnosticMessage : DiagnosticMessage
{
    public PrintableDiagnosticMessage(string message) : base(message)
    {
    }
    
    public override string ToString() => Message;
}
