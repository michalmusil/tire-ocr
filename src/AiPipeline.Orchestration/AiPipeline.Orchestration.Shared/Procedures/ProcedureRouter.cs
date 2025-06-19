using System.Reflection;
using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Contracts.Schema;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Procedures;

public class ProcedureRouter : IProcedureRouter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Func<IProcedure>> _procedureFactories = [];

    public ProcedureRouter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Manages routing and execution of a pipeline step.
    /// Finds a registered procedure matched based on the procedureId of current pipeline step and executes it
    /// with the current step input. Then it sends an asynchronous message to the next pipeline step (with output of
    /// the current step as input) if there are any next steps left, or invokes a pipeline finish strategy otherwise. 
    /// </summary>
    /// <param name="step">A pipeline step to process</param>
    /// <returns>A data result containing either the output of current pipeline step or failure</returns>
    public Task<DataResult<IApElement>> ProcessPipelineStep(RunPipelineStep step)
    {
        // TODO: route to local procedure, execute, route to next procedure, finish or publish error
        throw new NotImplementedException();
    }

    /// <summary>
    /// Scans provided assemblies for concrete implementations of IProcedure and registers factories for their construction.
    /// These factories are later used for routing pipeline steps to their matching Procedures.
    /// Only able to register procedures that have already been added to DI container.
    /// </summary>
    /// <param name="assemblies">Assemblies to scan for IProcedure implementors</param>
    /// <exception cref="ArgumentException">When provided assemblies are null or empty</exception>
    public void RegisterProceduresFromAssemblies(params Assembly[] assemblies)
    {
        if (assemblies is null || assemblies.Length == 0)
            throw new ArgumentException("Assemblies cannot be null or empty.");

        var procedureTypes = assemblies.SelectMany(a => a.GetTypes())
            .Where(type => typeof(IProcedure).IsAssignableFrom(type) &&
                           !type.IsInterface &&
                           !type.IsAbstract &&
                           !type.ContainsGenericParameters);

        foreach (var procedureType in procedureTypes)
        {
            string? procedureName = null;
            try
            {
                var tempProcedureInstance = (IProcedure?)_serviceProvider.GetService(procedureType);

                if (tempProcedureInstance is null)
                {
                    Console.Error.WriteLine(
                        $"Warning: Could not resolve a temporary instance of '{procedureType.FullName}' to get its 'Name'. " +
                        "This typically means the type is not registered with the DI container or its dependencies are not met during startup. " +
                        $"Skipping this procedure type from automatic registration into {nameof(ProcedureRouter)}.");
                    continue;
                }

                procedureName = tempProcedureInstance.Name;

                if (string.IsNullOrWhiteSpace(procedureName))
                {
                    Console.Error.WriteLine(
                        $"Warning: Procedure type '{procedureType.FullName}' has an invalid (null or empty) 'Name' property. Skipping.");
                    continue;
                }

                if (_procedureFactories.ContainsKey(procedureName))
                {
                    Console.Error.WriteLine(
                        $"Warning: A procedure with the name '{procedureName}' is already registered with {nameof(ProcedureRouter)}. " +
                        $"This means that multiple procedures with the same Name property exist within specified assemblies. " +
                        $"Skipping '{procedureType.FullName}'.");
                    continue;
                }

                var factory = () =>
                {
                    var instance = _serviceProvider.GetService(procedureType);
                    if (instance == null)
                    {
                        throw new InvalidOperationException(
                            $"Failed to resolve instance of type '{procedureType.FullName}' from service provider during execution. " +
                            $"This typically means that the {procedureType.FullName} has not been registered with the DI container.");
                    }

                    return (IProcedure)instance;
                };

                _procedureFactories.Add(procedureName, factory);
                Console.WriteLine($"Registered procedure: '{procedureName}' of type '{procedureType.FullName}'.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Error during registration of procedure type '{procedureType.FullName}' (Name: {procedureName ?? "N/A"}): {ex.Message}");
            }
        }
    }
}