using Microsoft.EntityFrameworkCore;
using SecretsSharingTool.Core.Models;

namespace SecretsSharingTool.Data;

public class AppUnitOfWork : DbContext
{
    public AppUnitOfWork(DbContextOptions<AppUnitOfWork> options) : base(options)
    {
    }

    public DbSet<Secret> Secrets { get; set; }
    public DbSet<SecretAccessAudit> AuditRecords { get; set; }
}
