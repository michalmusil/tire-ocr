using System.Reflection;
using AiPipeline.Orchestration.Shared.Procedures;
using Microsoft.Extensions.DependencyInjection;

namespace AiPipeline.Orchestration.Shared.Extensions;

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
            var procedureRouter = new ProcedureRouter(provider);
            procedureRouter.RegisterProceduresFromAssemblies(assemblies);
            return procedureRouter;
        });
    }
}