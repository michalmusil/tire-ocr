using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Providers;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Services;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Providers;
using AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Services;
using AiPipeline.Orchestration.Runner.Infrastructure.PipelineResult.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AiPipeline.Orchestration.Runner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        AddRepositories(services);
        AddProviders(services);
        AddServices(services);
        return services;
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<INodeTypeRepository, NodeTypeRepositoryFake>();
        services.AddScoped<IPipelineResultRepository, PipelineResultRepositoryFake>();
    }

    private static void AddProviders(IServiceCollection services)
    {
        services.AddScoped<IPipelineBuilderProvider, PipelineBuilderProvider>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IPipelinePublisherService, PipelineRabbitMqPublisherService>();
    }
}