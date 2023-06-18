using SecretsSharingTool.Core.Models;

namespace SecretsSharingTool.Data.Tests.Models;

public class SecretAccessAuditTests
{
    [Fact]
    public void SecretAccessAudit_WhenCreated_PopulatesAuditableFields()
    {
        var now = DateTimeOffset.UtcNow;
        var secret = new SecretAccessAudit();
        secret.Id.Should().NotBeEmpty();
        secret.CreatedOn.Should().BeOnOrAfter(now);
        secret.UpdatedOn.Should().BeOnOrAfter(now);
    }
}
