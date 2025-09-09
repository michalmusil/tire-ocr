namespace AiPipeline.Orchestration.FileService.Application.File.Dtos;

public record GetFileWithDataStreamDto(GetFileDto File, Stream DataStream);