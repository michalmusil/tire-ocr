using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using TireOcr.Shared.Result;
using Wolverine;

namespace AiPipeline.Orchestration.Shared.Nodes.Procedures.ProcedureCompletion;

public interface IProcedureCompletionStrategy
{
    public Task<Result> Execute(IMessageBus bus, RunPipelineStep processedStep, IApElement? result);
}