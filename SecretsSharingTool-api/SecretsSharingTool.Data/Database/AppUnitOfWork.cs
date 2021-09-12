using Microsoft.EntityFrameworkCore;
using SecretSharingTool.Data.Models;

namespace SecretSharingTool.Data.Database
{
    public sealed class AppUnitOfWork : DbContext
    {
        public AppUnitOfWork(DbContextOptions<AppUnitOfWork> options)
            : base(options)
        {
        }
        
        public DbSet<Secret> Secrets { get; set; }
    }
}