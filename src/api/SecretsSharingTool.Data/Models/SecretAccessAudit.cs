using SecretsSharingTool.Core.Models.Shared;

namespace SecretsSharingTool.Core.Models;

public class SecretAccessAudit : AuditableEntity
{
    public Guid SecretId { get; set; }
    public bool IsSuccessful => FailureReason == null;
    public FailureReason? FailureReason { get; set; }
    public string ClientIpAddress { get; set; }
    public string ClientUserAgent { get; set; }
}

public enum FailureReason
{
    NotFound = 10,
    SecretInactive,
    SecretExpired,
    
    DecryptionFailed = 20,
    NumberOfAllowedAttemptsExceeded = 21,
}
