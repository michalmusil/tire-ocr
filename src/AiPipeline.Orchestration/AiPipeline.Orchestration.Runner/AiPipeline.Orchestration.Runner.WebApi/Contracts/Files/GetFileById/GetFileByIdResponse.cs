using AiPipeline.Orchestration.Runner.Application.File.Dtos;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Files.GetFileById;

public record GetFileByIdResponse(
    GetFileDto File
);