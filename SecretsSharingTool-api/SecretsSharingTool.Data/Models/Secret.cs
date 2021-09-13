using System;
using SecretSharingTool.Data.Models.Shared;

namespace SecretSharingTool.Data.Models
{
    public sealed class Secret : AuditableEntity
    {
        public byte[] Message { get; set; }
        public byte[] EncryptedSymmetricKey { get; set; }
        public byte[] Iv { get; set; }
        public byte[] SignedHash { get; set; }
        public DateTime ExpireDateTime { get; set; }
        public int NumberOfAttempts { get; set; } = 0;
    }
}