using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using TireOcr.Shared.Result;
using Wolverine;

namespace AiPipeline.Orchestration.Shared.Procedures.PipelineFailure;

public class DefaultPipelineFailureStrategy : IPipelineFailureStrategy
{
    public async Task<Result> Execute(IMessageBus bus, RunPipelineStep failedStep, Failure failure,
        Exception? exception)
    {
        throw new NotImplementedException();
    }
}