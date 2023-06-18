using CustomEnvironmentConfig;
using SecretsSharingTool.Core.Configuration;

namespace SecretsSharingTool.Core.Tests.Configuration;

public class DatabaseConfigurationTests
{
    private readonly DatabaseConfiguration _configuration;
    
    public DatabaseConfigurationTests()
    {
        _configuration = ConfigurationParser.ParsePosix<DatabaseConfiguration>("local.env", ConfigurationTypeEnum.PreferFile);
    }
    
    [Fact]
    public void Configuration_WhenParsed_SetsExpectedValues()
    {
        _configuration.MySql.Host.Should().Be("secrets-sharing-db-mysql");
        _configuration.MySql.Port.Should().Be("3306");
        _configuration.MySql.User.Should().Be("secret-user");
        _configuration.MySql.Password.Should().Be("HelloWorld123");
        _configuration.MySql.Database.Should().Be("secrets-sharing-db");
    }
    
    [Fact]
    public void BuildConnectionString_WhenIsLocalIsFalse_ReturnsExpectedString()
    {
        var connectionString = _configuration.BuildConnectionString(false);
        connectionString.Should()
            .Be("server=secrets-sharing-db-mysql;uid=secret-user;password=HelloWorld123;database=secrets-sharing-db");
    }
    
    [Fact]
    public void BuildConnectionString_WhenIsLocalIsTrue_ReturnsExpectedString()
    {
        var connectionString = _configuration.BuildConnectionString(true);
        connectionString.Should()
            .Be("server=localhost;uid=secret-user;password=HelloWorld123;database=secrets-sharing-db");
    }
}