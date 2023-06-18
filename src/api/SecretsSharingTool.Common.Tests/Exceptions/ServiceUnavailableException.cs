namespace SecretsSharingTool.Common.Tests.Exceptions;

public class ServiceUnavailableException : Exception
{
    public ServiceUnavailableException(string message) : base(message)
    {
        
    }

    public ServiceUnavailableException()
    {
        
    }
}
