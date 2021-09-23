using System;
using SecretSharingTool.Data.Models.Shared;

namespace SecretsSharingTool.Core.Shared
{
    public static class Extensions
    {
        public static void SetCreatedAndActive(this AuditableEntity entity)
        {
            entity.CreatedOn = DateTime.UtcNow;
            entity.IsActive = true;
        }
        
        public static void SetModifiedAndInactive(this AuditableEntity entity)
        {
            entity.ModifiedOn = DateTime.UtcNow;
            entity.IsActive = false;
        }
    }
}