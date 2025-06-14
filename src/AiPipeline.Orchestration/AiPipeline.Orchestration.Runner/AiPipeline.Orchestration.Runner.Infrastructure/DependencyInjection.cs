using AiPipeline.Orchestration.Runner.Application.Repositories;
using AiPipeline.Orchestration.Runner.Application.Services;
using AiPipeline.Orchestration.Runner.Infrastructure.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.Services;
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