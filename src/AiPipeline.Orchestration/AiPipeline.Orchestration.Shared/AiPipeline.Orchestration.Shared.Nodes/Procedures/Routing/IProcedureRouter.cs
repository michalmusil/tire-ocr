using System.Reflection;
using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using AiPipeline.Orchestration.Shared.Nodes.Procedures.PipelineFailure;
using AiPipeline.Orchestration.Shared.Nodes.Procedures.ProcedureCompletion;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Nodes.Procedures.Routing;

public interface IProcedureRouter
{
    public IProcedureCompletionStrategy DefaultCompletionStrategy { get; set; }
    public IPipelineFailureStrategy DefaultFailureStrategy { get; set; }

    public Task<DataResult<IApElement>> ProcessPipelineStep(RunPipelineStep stepDescription);

    public void AddCompletionStrategyForProcedureType<T>(IProcedureCompletionStrategy strategy)
        where T : IProcedure;

    public void AddFailureStrategyForProcedureType<T>(IPipelineFailureStrategy strategy)
        where T : IProcedure;

    public void RegisterProceduresFromAssemblies(params Assembly[] assemblies);
}