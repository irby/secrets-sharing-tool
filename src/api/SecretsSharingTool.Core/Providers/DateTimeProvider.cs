using SecretsSharingTool.Core.Interfaces;

namespace SecretsSharingTool.Core.Providers;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset GetCurrentDateTimeOffset() => DateTimeOffset.UtcNow;

    public DateTime GetCurrentDateTime() => DateTime.UtcNow;
}