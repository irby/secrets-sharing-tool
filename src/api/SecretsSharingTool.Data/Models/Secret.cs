using SecretsSharingTool.Core.Models.Shared;

namespace SecretsSharingTool.Core.Models;

public class Secret : AuditableEntity
{
    public byte[]? EncryptedMessage { get; set; }
    public byte[]? EncryptedSymmetricKey { get; set; }
    public byte[] Iv { get; set; } = { new () };
    public long ExpiryMinutes { get; set; }
    public bool IsActive { get; set; } = true;
    public int NumberOfFailedAccesses { get; set; } = 0;

    public void Deactivate()
    {
        EncryptedMessage = null;
        IsActive = false;
        UpdatedOn = DateTimeOffset.UtcNow;
    }
}
