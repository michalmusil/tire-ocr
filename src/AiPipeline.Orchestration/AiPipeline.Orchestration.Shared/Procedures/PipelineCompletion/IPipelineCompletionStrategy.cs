using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Contracts.Schema;
using TireOcr.Shared.Result;
using Wolverine;

namespace AiPipeline.Orchestration.Shared.Procedures.PipelineCompletion;

public interface IPipelineCompletionStrategy
{
    public Task<Result> Execute(IMessageBus bus, RunPipelineStep processedStep, IApElement? result);
}