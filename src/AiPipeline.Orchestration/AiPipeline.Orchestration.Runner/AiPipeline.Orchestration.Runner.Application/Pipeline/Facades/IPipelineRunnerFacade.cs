using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Facades;

public interface IPipelineRunnerFacade
{
    public Task<DataResult<Domain.PipelineAggregate.Pipeline>> RunSinglePipelineAsync(RunPipelineDto runDto);

    public Task<DataResult<Domain.PipelineAggregate.PipelineBatch>> RunPipelineBatchAsync(RunPipelineBatchDto runDto);
}