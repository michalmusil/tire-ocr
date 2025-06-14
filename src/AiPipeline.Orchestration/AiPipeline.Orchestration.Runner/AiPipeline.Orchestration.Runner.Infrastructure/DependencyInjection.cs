using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Services;
using AiPipeline.Orchestration.Runner.Infrastructure.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AiPipeline.Orchestration.Runner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        AddRepositories(services);
        AddServices(services);
        return services;
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IPipelineBuilderService, PipelineBuilderService>();
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<INodeTypeRepository, NodeTypeRepositoryFake>();
    }
}