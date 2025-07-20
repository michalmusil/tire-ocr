using System.Reflection;
using AiPipeline.Orchestration.Shared.Nodes.Procedures;
using AiPipeline.Orchestration.Shared.Nodes.Procedures.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wolverine.Runtime;

namespace AiPipeline.Orchestration.Shared.Nodes.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddProcedureRoutingFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies is null || assemblies.Length == 0)
            throw new ArgumentException("Assemblies cannot be null or empty.");

        var procedureTypes = assemblies.SelectMany(a => a.GetTypes())
            .Where(type => typeof(IProcedure).IsAssignableFrom(type) &&
                           !type.IsInterface &&
                           !type.IsAbstract &&
                           !type.ContainsGenericParameters);

        foreach (var procedureType in procedureTypes)
            services.AddTransient(procedureType);

        services.AddSingleton<IProcedureRouter>(provider =>
        {
            var wolverineRuntime = provider.GetRequiredService<IWolverineRuntime>();
            var messageBus = new MessageBus(wolverineRuntime);
            var logger = provider.GetRequiredService<ILogger<ProcedureRouter>>();
            var procedureRouter = new ProcedureRouter(messageBus, provider, logger);

            procedureRouter.RegisterProceduresFromAssemblies(assemblies);
            return procedureRouter;
        });
    }
}