using AiPipeline.Orchestration.Contracts.Schema;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Pipelines.RunAsync;

public record RunPipelineAsyncRequest(
    IApElement Input,
    List<RunPipelineStepDto> Steps
    // IEnumerable<IFormFile> InputFiles
);