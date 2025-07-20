using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using TireOcr.Shared.Result;
using Wolverine;

namespace AiPipeline.Orchestration.Shared.Nodes.Procedures.PipelineFailure;

public interface IPipelineFailureStrategy
{
    public Task<Result> Execute(IMessageBus bus, RunPipelineStep failedStep, Failure failure,
        Exception? exception = null);
}