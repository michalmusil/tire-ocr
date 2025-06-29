using AiPipeline.Orchestration.Runner.Application.File.Dtos;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Files.UploadFile;

public record UploadFileResponse(
    GetFileDto File
);