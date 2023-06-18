namespace SecretsSharingTool.Data.Database;

public interface IDatabaseAccessor
{
    Task<bool> CanConnect();
}
