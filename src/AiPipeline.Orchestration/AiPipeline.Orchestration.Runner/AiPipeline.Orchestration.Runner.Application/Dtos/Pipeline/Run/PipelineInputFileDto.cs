namespace AiPipeline.Orchestration.Runner.Application.Dtos.Pipeline.Run;

public record PipelineInputFileDto(
    string FileName,
    string ContentType,
    byte[] Data
);