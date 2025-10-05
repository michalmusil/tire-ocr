using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.DataAccess;

public class EvaluationToolDbContextFactory : IDesignTimeDbContextFactory<EvaluationToolDbContext>
{
    public EvaluationToolDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("AiPipelineEvaluationToolDb");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                $"{nameof(EvaluationToolDbContextFactory)}: Default connection string was not found in configuration.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<EvaluationToolDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new EvaluationToolDbContext(optionsBuilder.Options);
    }
}