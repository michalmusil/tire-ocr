using System.Reflection;
using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using AiPipeline.Orchestration.Shared.Nodes.Procedures.PipelineFailure;
using AiPipeline.Orchestration.Shared.Nodes.Procedures.ProcedureCompletion;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using Wolverine;

namespace AiPipeline.Orchestration.Shared.Nodes.Procedures.Routing;

public class ProcedureRouter : IProcedureRouter
{
    private readonly IMessageBus _bus;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProcedureRouter> _logger;

    private readonly Dictionary<string, Func<IProcedure>> _procedureFactories;
    private readonly Dictionary<Type, IProcedureCompletionStrategy> _completionStrategies;
    private readonly Dictionary<Type, IPipelineFailureStrategy> _failureStrategies;

    public IProcedureCompletionStrategy DefaultCompletionStrategy { get; set; }
    public IPipelineFailureStrategy DefaultFailureStrategy { get; set; }

    public ProcedureRouter(IMessageBus bus, IServiceProvider serviceProvider, ILogger<ProcedureRouter> logger)
    {
        _bus = bus;
        _serviceProvider = serviceProvider;
        _logger = logger;

        _procedureFactories = new();
        _completionStrategies = new();
        _failureStrategies = new();

        DefaultCompletionStrategy = new DefaultProcedureCompletionStrategy();
        DefaultFailureStrategy = new DefaultPipelineFailureStrategy();
        RegisterProceduresFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
    }

    /// <summary>
    /// Manages routing and execution of a pipeline step.
    /// Finds a registered procedure matched based on the procedureId of current pipeline step and executes it
    /// with the current step input. Then it sends an asynchronous message to the next pipeline step (with output of
    /// the current step as input) if there are any next steps left and invokes a procedure completion strategy. 
    /// </summary>
    /// <param name="stepDescription">A pipeline step to process</param>
    /// <returns>A data result containing either the output of current pipeline step or failure</returns>
    public async Task<DataResult<IApElement>> ProcessPipelineStep(RunPipelineStep stepDescription)
    {
        var input = stepDescription.CurrentStepInput;
        var fileReferences = stepDescription.FileReferences;
        var currentStep = stepDescription.CurrentStep;

        var currentStepProcedureFactory =
            _procedureFactories.GetValueOrDefault(stepDescription.CurrentStep.ProcedureId);
        if (currentStepProcedureFactory is null)
            return DataResult<IApElement>.NotFound($"Procedure with id: {currentStep.ProcedureId} is not registered");

        IProcedure? procedure = null;
        DataResult<IApElement> result;
        try
        {
            var currentProcedure = currentStepProcedureFactory();
            procedure = currentProcedure;
            result = await currentProcedure.ExecuteAsync(input, fileReferences);
        }
        catch (InvalidOperationException ex)
        {
            var failedResult = DataResult<IApElement>
                .NotFound($"Procedure with id: {currentStep.ProcedureId} is not registered");
            await DefaultFailureStrategy.Execute(_bus, stepDescription, failedResult.PrimaryFailure!, ex);
            return failedResult;
        }
        catch (Exception ex)
        {
            var failure = new Failure(500, $"Failed to execute procedure {currentStep.ProcedureId}");
            var strategy = procedure is null
                ? DefaultFailureStrategy
                : GetProcedureFailureStrategy(procedure);
            await strategy.Execute(_bus, stepDescription, failure, ex);
            return DataResult<IApElement>.Failure(failure);
        }

        if (result.IsFailure)
        {
            var failureStrategy = GetProcedureFailureStrategy(procedure);
            await failureStrategy.Execute(_bus, stepDescription, result.PrimaryFailure!);
            return result;
        }

        var nextStepProcedureIdentifier = stepDescription.NextSteps.FirstOrDefault();
        var pipelineIsCompleted = nextStepProcedureIdentifier is null;
        if (!pipelineIsCompleted)
        {
            var followingStepsProcedureIdentifiers = stepDescription.NextSteps
                .Skip(1)
                .ToList();

            var nextStepMessage = new RunPipelineStep(
                PipelineId: stepDescription.PipelineId,
                CurrentStep: nextStepProcedureIdentifier!,
                CurrentStepInput: result.Data!,
                NextSteps: followingStepsProcedureIdentifiers,
                FileReferences: fileReferences
            );
            await _bus.PublishAsync(nextStepMessage);
        }

        var completionStrategy = GetProcedureCompletionStrategy(procedure);
        await completionStrategy.Execute(_bus, stepDescription, result.Data!);
        return result;
    }

    /// <summary>
    /// Registers a completion strategy that will be used over the default completion strategy when the pipeline
    /// is completed (has no more steps to route), but only for the specified procedure type.
    /// </summary>
    /// <param name="strategy">Completion strategy to be used</param>
    /// <typeparam name="T">Concrete type of procedure for which the strategy should be used</typeparam>
    public void AddCompletionStrategyForProcedureType<T>(IProcedureCompletionStrategy strategy) where T : IProcedure
        => _completionStrategies[typeof(T)] = strategy;

    /// <summary>
    /// Registers a failure strategy that will be used over the default failure strategy when the step
    /// execution fails, but only for the specified procedure type.
    /// </summary>
    /// <param name="strategy">Failure strategy to be used</param>
    /// <typeparam name="T">Concrete type of procedure for which the strategy should be used</typeparam>
    public void AddFailureStrategyForProcedureType<T>(IPipelineFailureStrategy strategy) where T : IProcedure
        => _failureStrategies[typeof(T)] = strategy;

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

    private IProcedureCompletionStrategy GetProcedureCompletionStrategy(IProcedure procedure) =>
        _completionStrategies.ContainsKey(procedure.GetType())
            ? _completionStrategies[procedure.GetType()]
            : DefaultCompletionStrategy;

    private IPipelineFailureStrategy GetProcedureFailureStrategy(IProcedure procedure) =>
        _failureStrategies.ContainsKey(procedure.GetType())
            ? _failureStrategies[procedure.GetType()]
            : DefaultFailureStrategy;
}