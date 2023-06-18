namespace SecretsSharingtool.Data.Database;

public class DatabaseAccessor : IDatabaseAccessor
{
    private readonly AppUnitOfWork _appUnitOfWork;
    public DatabaseAccessor(AppUnitOfWork appUnitOfWork)
    {
        _appUnitOfWork = appUnitOfWork;
    }

    public async Task<bool> CanConnect()
    {
        return await _appUnitOfWork.Database.CanConnectAsync();
    }
}