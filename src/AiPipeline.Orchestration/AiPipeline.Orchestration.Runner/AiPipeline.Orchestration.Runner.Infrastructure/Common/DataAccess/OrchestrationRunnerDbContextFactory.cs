using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess;

public class OrchestrationRunnerDbContextFactory : IDesignTimeDbContextFactory<OrchestrationRunnerDbContext>
{
    public OrchestrationRunnerDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("AiPipelineDb");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                $"{nameof(OrchestrationRunnerDbContextFactory)}: Default connection string was not found in configuration.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<OrchestrationRunnerDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new OrchestrationRunnerDbContext(optionsBuilder.Options);
    }
}