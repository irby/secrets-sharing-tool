using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecretsSharingTool.Data.Database;

namespace SecretsSharingTool.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppUnitOfWork>(opt => opt.UseMySQL(connectionString));
        return services;
    }

    public static IServiceCollection AddDatabaseAccessor(this IServiceCollection services)
    {
        services.AddTransient<IDatabaseAccessor, DatabaseAccessor>();
        return services;
    }

    public static void ApplyPendingMigrations(this IServiceCollection services)
    {
        var numAttempts = 0;
        var allowedAttempts = 10;
        Exception exception = null;
        
        var traceId = Guid.NewGuid();
        var provider = services.BuildServiceProvider();
        var db = provider.GetService<AppUnitOfWork>()!;
        var logger = provider.GetService<ILogger<AppUnitOfWork>>()!;
        
        while (numAttempts < allowedAttempts)
        {
            try
            {
                var pendingMigrations = db.Database.GetPendingMigrations().ToList();
        
                LogMessageWithTraceId(logger, $"{pendingMigrations.Count} database migration(s) waiting to be applied.", traceId);
        
                if (!pendingMigrations.Any())
                {
                    LogMessageWithTraceId(logger, "No migrations will be applied.", traceId);
                    return;
                }

                LogMessageWithTraceId(logger, $"Applying {pendingMigrations.Count} migration(s) to the database...", traceId);
                LogMessageWithTraceId(logger, string.Join(", ", pendingMigrations), traceId);

                db.Database.Migrate();
                LogMessageWithTraceId(logger, "Database migration(s) have been successfully applied.", traceId);
                
                return;
            }
            catch (Exception ex)
            {
                exception = ex;
                Thread.Sleep(1000);
                numAttempts++;
            }
        }
        
        
        logger.LogCritical(exception, $"An error occurred while applying migration. \nError: {exception!.Message}. \nTrace ID: {traceId}");
    }

    private static void LogMessageWithTraceId(ILogger logger, string message, Guid traceId)
    {
        logger.LogInformation($"{message}\tTrace ID: {traceId}");
    }
}
