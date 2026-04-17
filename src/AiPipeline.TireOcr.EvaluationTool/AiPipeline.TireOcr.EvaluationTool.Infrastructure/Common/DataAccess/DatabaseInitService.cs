using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.DataAccess;

public class DatabaseInitService : BackgroundService
{
    private readonly IDbContextFactory<EvaluationToolDbContext> _dbContextFactory;
    private readonly ILogger<DatabaseInitService> _logger;

    public DatabaseInitService(IDbContextFactory<EvaluationToolDbContext> dbContextFactory,
        ILogger<DatabaseInitService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(stoppingToken);
        try
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync(stoppingToken);
            _logger.LogInformation("Ensuring database is migrated...");
            await context.Database.MigrateAsync(stoppingToken);
            await SeedAsync(context, pendingMigrations, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    private async Task SeedAsync(EvaluationToolDbContext context, IEnumerable<string> pendingMigrations,
        CancellationToken stoppingToken)
    {
        var isFirstMigration = pendingMigrations.Contains("20251005185443_InitialMigration");
        if (isFirstMigration)
        {
            _logger.LogInformation("Seeding initial user data");
            context.Users.Add(new Domain.UserAggregate.User("admin", "$2a$13$f6FCH8Kvbg5uPLzUpyBl6e4P8HTo040Tl27Ik5xesmOPpcjR4.PMK"));
            await context.SaveChangesAsync(stoppingToken);
        }
    }
}