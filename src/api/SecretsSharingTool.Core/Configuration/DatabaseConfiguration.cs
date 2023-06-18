namespace SecretsSharingTool.Core.Configuration;

public class DatabaseConfiguration
{
    public MySql MySql { get; set; } = new();

    public string BuildConnectionString(bool isLocal = false)
    {
        var host = isLocal ? "localhost" : MySql.Host;
        return $"server={host};uid={MySql.User};password={MySql.Password};database={MySql.Database}";
    }
}

public class MySql
{
    public string Host { get; set; }
    public string Port { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public string Database { get; set; }
}