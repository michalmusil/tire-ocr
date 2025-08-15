using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Services;

public interface IPipelinePublisherService
{
    public Task<Result> PublishAsync(Domain.PipelineAggregate.Pipeline pipeline, IApElement input);
    public Task<Result> PublishManyAsync(Dictionary<Domain.PipelineAggregate.Pipeline, IApElement> pipelines);
}