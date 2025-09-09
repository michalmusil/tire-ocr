using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AiPipeline.Orchestration.FileService.Infrastructure.Common.DataAccess;

public class FileServiceDbContextFactory : IDesignTimeDbContextFactory<FileServiceDbContext>
{
    public FileServiceDbContext CreateDbContext(string[] args)
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
                $"{nameof(FileServiceDbContextFactory)}: Default connection string was not found in configuration.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<FileServiceDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new FileServiceDbContext(optionsBuilder.Options);
    }
}