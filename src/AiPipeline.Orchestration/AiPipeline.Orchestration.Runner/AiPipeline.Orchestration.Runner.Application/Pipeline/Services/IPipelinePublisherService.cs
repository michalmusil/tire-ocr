using AiPipeline.Orchestration.Shared.Contracts.Schema;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Services;

public interface IPipelinePublisherService
{
    public Task<Result> PublishPipeline(Domain.PipelineAggregate.Pipeline pipeline, IApElement input);
}