using AiPipeline.Orchestration.FileService.Application.File.Dtos;

namespace AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetFileById;

public record GetFileByIdResponse(
    GetFileDto File
);