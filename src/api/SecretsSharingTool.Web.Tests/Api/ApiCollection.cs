namespace SecretsSharingTool.Web.Tests.Api;

[CollectionDefinition(Name)]
public abstract class ApiCollection : ICollectionFixture<ApiFixture>
{
    public const string Name = "Api";
}