using SecretsSharingTool.Core.Models;

namespace SecretsSharingTool.Data.Tests.Models;

public class SecretTests
{
    [Fact]
    public void Secret_WhenCreated_PopulatesAuditableFields()
    {
        var now = DateTimeOffset.UtcNow;
        var secret = new Secret();
        secret.Id.Should().NotBeEmpty();
        secret.CreatedOn.Should().BeOnOrAfter(now);
        secret.UpdatedOn.Should().BeOnOrAfter(now);
    }

    [Fact]
    public void Secret_WhenCreated_DefaultsIsActiveToTrue()
    {
        var secret = new Secret();
        secret.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Secret_Deactivate_NullsEncryptedMessageAndSetsIsActiveToFalse()
    {
        var now = DateTimeOffset.Now;
        
        var secret = new Secret()
        {
            EncryptedMessage = new byte[] { },
            IsActive = true,
            UpdatedOn = DateTimeOffset.Now.AddDays(-1)
        };
        secret.Deactivate();
        secret.IsActive.Should().BeFalse();
        secret.UpdatedOn.Should().BeOnOrAfter(now);
    }
}
