using AiPipeline.Orchestration.FileService.Application.File.Dtos;

namespace AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.UploadFile;

public record UploadFileResponse(
    GetFileDto File
);