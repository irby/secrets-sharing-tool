using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecretsSharingtool.Data.Database;

namespace SecretsSharingtool.Data;

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
        var traceId = Guid.NewGuid();
        var provider = services.BuildServiceProvider();
        var db = provider.GetService<AppUnitOfWork>()!;
        var logger = provider.GetService<ILogger<AppUnitOfWork>>()!;
        var pendingMigrations = db.Database.GetPendingMigrations().ToList();
        LogMessageWithTraceId(logger, $"{pendingMigrations.Count} database migration(s) waiting to be applied.", traceId);
        
        if (!pendingMigrations.Any())
        {
            LogMessageWithTraceId(logger, "No migrations will be applied.", traceId);
            return;
        }

        LogMessageWithTraceId(logger, $"Applying {pendingMigrations.Count} migration(s) to the database...", traceId);
        LogMessageWithTraceId(logger, string.Join(", ", pendingMigrations), traceId);

        try
        {
            db.Database.Migrate();
            LogMessageWithTraceId(logger, "Database migration(s) have been successfully applied.", traceId);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, $"An error occurred while applying migration. \nError: {ex.Message}. \nTrace ID: {traceId}");
            throw;
        }
    }

    private static void LogMessageWithTraceId(ILogger logger, string message, Guid traceId)
    {
        logger.LogInformation($"{message}\tTrace ID: {traceId}");
    }
}