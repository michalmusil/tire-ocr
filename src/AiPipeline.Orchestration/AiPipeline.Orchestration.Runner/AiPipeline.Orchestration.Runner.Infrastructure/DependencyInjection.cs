using AiPipeline.Orchestration.FileService.GrpcSdk;
using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Facades;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Providers;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Services;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using AiPipeline.Orchestration.Runner.Application.User.Repositories;
using AiPipeline.Orchestration.Runner.Application.User.Services;
using AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Infrastructure.File.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Facades;
using AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Providers;
using AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Services;
using AiPipeline.Orchestration.Runner.Infrastructure.PipelineResult.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.User.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.User.Services;
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
        AddFacades(services);
        AddGrpcFileService(services);
        AddDbContext(services, configuration);
        return services;
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<INodeTypeEntityRepository, NodeTypeEntityRepository>();
        services.AddScoped<IPipelineResultEntityRepository, PipelineResultEntityRepository>();
        services.AddScoped<IFileRepository, FileRepositoryGrpc>();
        services.AddScoped<IUserEntityRepository, UserEntityRepository>();
        services.AddScoped<IRefreshTokenEntityRepository, RefreshTokenEntityRepository>();
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
        services.AddScoped<IHashService, HashService>();
        services.AddScoped<IAuthService, AuthService>();
    }

    private static void AddFacades(IServiceCollection services)
    {
        services.AddScoped<IPipelineRunnerFacade, PipelineRunnerFacade>();
    }

    private static void AddGrpcFileService(IServiceCollection services)
    {
        services.AddFileServiceSdk(new Uri("http://FileService"));
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AiPipelineDb");
        services.AddDbContextFactory<OrchestrationRunnerDbContext>(options => { options.UseNpgsql(connectionString); });
    }
}