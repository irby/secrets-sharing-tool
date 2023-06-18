namespace SecretsSharingTool.Core.Handlers.Secret.Create;

public class CreateSecretCommandResponse
{
    public Guid SecretId { get; set; }
    public string Key { get; set; }
    public DateTimeOffset ExpireDateTime { get; set; }
    public long ExpireDateTimeEpoch => ExpireDateTime.ToUnixTimeSeconds();
    
}
