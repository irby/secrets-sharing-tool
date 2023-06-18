namespace SecretsSharingTool.Core.Tests.Database;

[CollectionDefinition(Name)]
public abstract class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    public const string Name = "Database";
}