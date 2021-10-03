using System;

namespace SecretsSharingTool.Core.Create
{
    public sealed class SecretCreationCommandHandlerResponse
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public DateTimeOffset ExpireDateTime { get; set; }
    }
}