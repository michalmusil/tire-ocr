using AiPipeline.Orchestration.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;

public record RunPipelineDto(
    IApElement Input,
    IEnumerable<PipelineInputFileDto> InputFiles,
    List<RunPipelineStepDto> Steps
);