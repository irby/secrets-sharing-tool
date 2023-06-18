namespace SecretsSharingtool.Data.Database;

public interface IDatabaseAccessor
{
    Task<bool> CanConnect();
}