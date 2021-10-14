namespace SecretsSharingTool.Api.Queue
{
    public abstract class BackgroundService : BackgroundThread
    {
        protected BackgroundService(string name) : base(name)
        {
        }
    }
}