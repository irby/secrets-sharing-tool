namespace SecretsSharingTool.Core.Interfaces;

public interface IDateTimeProvider
{
    public DateTimeOffset GetCurrentDateTimeOffset();
    public DateTime GetCurrentDateTime();
}