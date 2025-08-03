using AiPipeline.Orchestration.FileService.Application.File.Repositories;
using AiPipeline.Orchestration.FileService.Infrastructure.Common.DataAccess;
using AiPipeline.Orchestration.FileService.Infrastructure.File.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace AiPipeline.Orchestration.FileService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddRepositories(services);
        AddS3Storage(services, configuration);
        AddDbContext(services, configuration);
        return services;
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IFileStorageProviderRepository, FileStorageMinioRepository>();
    }

    private static void AddS3Storage(IServiceCollection services, IConfiguration configuration)
    {
        // Minio client doesn't support base addresses starting with protocol
        var uri = configuration
            .GetValue<string>("services:minio:api:0")
            ?.Replace("http://", "")
            .Replace("https://", "");
        var username = configuration.GetValue<string>("MINIO_USERNAME");
        var password = configuration.GetValue<string>("MINIO_PASSWORD");

        services.AddMinio(configureClient => configureClient
            .WithEndpoint(uri)
            .WithCredentials(username, password)
            .WithSSL(false)
            .Build()
        );
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AiPipelineDb");
        services.AddDbContextFactory<FileServiceDbContext>(options => { options.UseNpgsql(connectionString); });
    }
}