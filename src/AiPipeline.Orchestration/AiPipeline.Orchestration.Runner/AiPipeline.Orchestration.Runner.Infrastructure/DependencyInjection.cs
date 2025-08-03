using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Providers;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Services;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Infrastructure.File.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Providers;
using AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Services;
using AiPipeline.Orchestration.Runner.Infrastructure.PipelineResult.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace AiPipeline.Orchestration.Runner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddRepositories(services);
        AddUnitOfWork(services);
        AddProviders(services);
        AddServices(services);
        AddS3Storage(services, configuration);
        AddDbContext(services, configuration);
        return services;
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<INodeTypeRepository, NodeTypeRepository>();
        services.AddScoped<IPipelineResultRepository, PipelineResultRepository>();
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IFileStorageProviderRepository, FileStorageMinioRepository>();
    }

    public static void AddUnitOfWork(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddProviders(IServiceCollection services)
    {
        services.AddScoped<IPipelineBuilderProvider, PipelineBuilderProvider>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IPipelineResultSubscriberService, PipelineResultSubscriberService>();
        services.AddScoped<IPipelinePublisherService, PipelineRabbitMqPublisherService>();
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
        services.AddDbContextFactory<OrchestrationRunnerDbContext>(options => { options.UseNpgsql(connectionString); });
    }
}