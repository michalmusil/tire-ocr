using AiPipeline.Orchestration.Runner.Application.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AiPipeline.Orchestration.Runner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        AddRepositories(services);
        return services;
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<INodeTypeRepository, NodeTypeRepositoryFake>();
    }
}