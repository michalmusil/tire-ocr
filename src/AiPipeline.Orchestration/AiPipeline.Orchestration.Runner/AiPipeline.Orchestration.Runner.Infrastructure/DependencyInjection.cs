using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Builders;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Providers;
using AiPipeline.Orchestration.Runner.Infrastructure.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Builders;
using AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace AiPipeline.Orchestration.Runner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        AddRepositories(services);
        AddProviders(services);
        return services;
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<INodeTypeRepository, NodeTypeRepositoryFake>();
    }

    private static void AddProviders(IServiceCollection services)
    {
        services.AddScoped<IPipelineBuilderProvider, PipelineBuilderProvider>();
    }
}