using AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Services;

public interface IPipelinePublisherService
{
    public Task<Result> PublishAsync(Domain.PipelineAggregate.Pipeline pipeline);
    public Task<Result> PublishBatchAsync(PipelineBatch batch);
}