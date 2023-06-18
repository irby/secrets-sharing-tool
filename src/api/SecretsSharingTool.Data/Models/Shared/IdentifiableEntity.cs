namespace SecretsSharingTool.Core.Models.Shared;

public abstract class IdentifiableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}