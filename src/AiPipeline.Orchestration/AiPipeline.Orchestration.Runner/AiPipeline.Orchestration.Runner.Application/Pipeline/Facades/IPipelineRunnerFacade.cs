using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Facades;

public interface IPipelineRunnerFacade
{
    public Task<DataResult<Domain.PipelineAggregate.Pipeline>> RunPipelineAsync(RunPipelineDto runDto);
}