using System.Reflection;
using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Contracts.Schema;
using AiPipeline.Orchestration.Shared.Procedures.PipelineCompletion;
using AiPipeline.Orchestration.Shared.Procedures.PipelineFailure;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Procedures.Routing;

public interface IProcedureRouter
{
    public IPipelineCompletionStrategy DefaultCompletionStrategy { get; set; }
    public IPipelineFailureStrategy DefaultFailureStrategy { get; set; }

    public Task<DataResult<IApElement>> ProcessPipelineStep(RunPipelineStep stepDescription);

    public void AddCompletionStrategyForProcedureType<T>(IPipelineCompletionStrategy strategy)
        where T : IProcedure;

    public void AddFailureStrategyForProcedureType<T>(IPipelineFailureStrategy strategy)
        where T : IProcedure;

    public void RegisterProceduresFromAssemblies(params Assembly[] assemblies);
}