namespace AiPipeline.Orchestration.Runner.Application.File.Dtos;

public record GetFileWithDataStreamDto(GetFileDto File, Stream DataStream);