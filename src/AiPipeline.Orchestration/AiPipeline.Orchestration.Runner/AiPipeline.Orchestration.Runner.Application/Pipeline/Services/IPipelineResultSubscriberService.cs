using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Services;

public interface IPipelineResultSubscriberService
{
    public Task<DataResult<Domain.PipelineResultAggregate.PipelineResult>> WaitForPipelineResultAsync(Guid pipelineId,
        TimeSpan timeout,
        CancellationToken? cancellationToken = null
    );

    public Task<Result> CompleteWithPipelineResultAsync(
        Domain.PipelineResultAggregate.PipelineResult pipelineResult);
}