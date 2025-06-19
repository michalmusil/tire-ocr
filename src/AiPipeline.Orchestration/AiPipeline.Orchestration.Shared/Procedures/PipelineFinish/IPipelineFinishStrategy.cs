using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Contracts.Schema;
using TireOcr.Shared.Result;
using Wolverine;

namespace AiPipeline.Orchestration.Shared.Procedures.PipelineFinish;

public interface IPipelineFinishStrategy
{
    public Task<Result> Execute(IMessageBus bus, RunPipelineStep processedStep, IApElement? result);
}