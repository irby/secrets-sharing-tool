using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SecretSharingTool.Data.Database;

namespace SecretsSharingTool.Api.Data
{
    public static class DataServices
    {
        public static void Setup(IServiceCollection serviceCollection, string connectionString)
        {
            serviceCollection.AddDbContext<AppUnitOfWork>(optionsBuilder =>
            {
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                    builder =>
                    {
                        builder.EnableRetryOnFailure();
                        builder.CommandTimeout(180);
                    }).EnableDetailedErrors();
            });
        }
    }
}