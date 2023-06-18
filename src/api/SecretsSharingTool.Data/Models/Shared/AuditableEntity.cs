namespace SecretsSharingTool.Core.Models.Shared;

public abstract class AuditableEntity : IdentifiableEntity
{
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.UtcNow;
}