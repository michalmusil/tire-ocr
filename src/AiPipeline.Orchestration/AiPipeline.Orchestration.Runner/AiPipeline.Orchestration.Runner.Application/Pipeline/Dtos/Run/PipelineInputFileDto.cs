namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;

public record PipelineInputFileDto(
    string FileName,
    string ContentType,
    byte[] Data
);