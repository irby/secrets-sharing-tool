using System;

namespace SecretSharingTool.Data.Models.Shared
{
    public abstract class AuditableEntity
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
    }
}