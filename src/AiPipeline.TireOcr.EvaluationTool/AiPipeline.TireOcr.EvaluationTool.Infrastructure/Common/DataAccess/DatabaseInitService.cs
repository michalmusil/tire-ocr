using AiPipeline.TireOcr.EvaluationTool.Application.User.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.DataAccess;

public class DatabaseInitService : BackgroundService
{
    private readonly IDbContextFactory<EvaluationToolDbContext> _dbContextFactory;
    private readonly IConfiguration _configuration;
    private readonly IHashService _hashService;
    private readonly ILogger<DatabaseInitService> _logger;

    public DatabaseInitService(IDbContextFactory<EvaluationToolDbContext> dbContextFactory,
        IConfiguration configuration, IHashService hashService, ILogger<DatabaseInitService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _configuration = configuration;
        _hashService = hashService;
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
            var initialUsername = _configuration.GetValue<string>("InitialUserCredentials:Username");
            var initialPassword = _configuration.GetValue<string>("InitialUserCredentials:Password");
            var credentialsSpecified = initialUsername?.Length > 0 && initialPassword?.Length >= 5;
            if (credentialsSpecified)
            {
                var passwordHashResult = _hashService.GetHashOf(initialPassword!);
                if (passwordHashResult.IsSuccess)
                {
                    _logger.LogInformation("Seeding initial user data");
                    context.Users.Add(new Domain.UserAggregate.User(initialUsername!, passwordHashResult.Data!));
                    await context.SaveChangesAsync(stoppingToken);
                }
            }
        }
    }
}