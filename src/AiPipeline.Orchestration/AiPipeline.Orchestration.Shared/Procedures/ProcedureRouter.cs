using System.Data;
using System.Reflection;
using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Contracts.Schema;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using Wolverine;

namespace AiPipeline.Orchestration.Shared.Procedures;

public class ProcedureRouter : IProcedureRouter
{
    private readonly IMessageBus _bus;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Func<IProcedure>> _procedureFactories;
    private readonly ILogger<ProcedureRouter> _logger;

    public ProcedureRouter(IMessageBus bus, IServiceProvider serviceProvider, ILogger<ProcedureRouter> logger)
    {
        _bus = bus;
        _serviceProvider = serviceProvider;
        _procedureFactories = new Dictionary<string, Func<IProcedure>>();
        _logger = logger;
    }

    /// <summary>
    /// Manages routing and execution of a pipeline step.
    /// Finds a registered procedure matched based on the procedureId of current pipeline step and executes it
    /// with the current step input. Then it sends an asynchronous message to the next pipeline step (with output of
    /// the current step as input) if there are any next steps left, or invokes a pipeline finish strategy otherwise. 
    /// </summary>
    /// <param name="stepDescription">A pipeline step to process</param>
    /// <returns>A data result containing either the output of current pipeline step or failure</returns>
    public async Task<DataResult<IApElement>> ProcessPipelineStep(RunPipelineStep stepDescription)
    {
        var hash = this.GetHashCode();
        var input = stepDescription.CurrentStepInput;
        var currentStep = stepDescription.CurrentStep;

        var currentStepProcedureFactory =
            _procedureFactories.GetValueOrDefault(stepDescription.CurrentStep.ProcedureId);
        if (currentStepProcedureFactory is null)
            return DataResult<IApElement>.NotFound($"Procedure with id: {currentStep.ProcedureId} is not registered");

        DataResult<IApElement> result;
        try
        {
            var currentProcedure = currentStepProcedureFactory();
            result = await currentProcedure.Execute(input);
        }
        catch (InvalidOperationException ex)
        {
            return DataResult<IApElement>.NotFound($"Procedure with id: {currentStep.ProcedureId} is not registered");
        }
        catch
        {
            return DataResult<IApElement>.Failure(new Failure(500,
                $"Failed to execute procedure {currentStep.ProcedureId}"));
        }

        if (result.IsFailure)
            return result;

        var nextStepProcedureIdentifier = stepDescription.NextSteps.FirstOrDefault();
        if (nextStepProcedureIdentifier is null)
            throw new RowNotInTableException("Implement executing pipeline finish strategy");

        var followingStepsProcedureIdentifiers = stepDescription.NextSteps
            .Skip(1)
            .ToList();

        var nextStepMessage = new RunPipelineStep(
            CurrentStep: nextStepProcedureIdentifier,
            CurrentStepInput: result.Data!,
            NextSteps: followingStepsProcedureIdentifiers
        );
        await _bus.PublishAsync(nextStepMessage);

        return result;
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
            string? procedureId = null;
            try
            {
                var tempProcedureInstance = (IProcedure?)_serviceProvider.GetService(procedureType);

                if (tempProcedureInstance is null)
                {
                    _logger.LogError(
                        $"Warning: Could not resolve a temporary instance of '{procedureType.FullName}' to get its 'Id'. " +
                        "This typically means the type is not registered with the DI container or its dependencies are not met during startup. " +
                        $"Skipping this procedure type from automatic registration into {nameof(ProcedureRouter)}.");
                    continue;
                }

                procedureId = tempProcedureInstance.Id;

                if (string.IsNullOrWhiteSpace(procedureId))
                {
                    _logger.LogError(
                        $"Warning: Procedure type '{procedureType.FullName}' has an invalid (null or empty) 'Id' property. Skipping.");
                    continue;
                }

                if (_procedureFactories.ContainsKey(procedureId))
                {
                    _logger.LogError(
                        $"Warning: A procedure with the Id '{procedureId}' is already registered with {nameof(ProcedureRouter)}. " +
                        $"This means that multiple procedures with the same 'Id' property exist within specified assemblies. " +
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

                _procedureFactories.Add(procedureId, factory);
                _logger.LogInformation($"Registered procedure: '{procedureId}' of type '{procedureType.FullName}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"Error during registration of procedure type '{procedureType.FullName}' (Id: {procedureId ?? "N/A"}): {ex.Message}");
            }
        }
    }
}