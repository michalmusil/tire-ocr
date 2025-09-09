using AiPipeline.Orchestration.FileService.Application.File.Dtos;

namespace AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetFilesByIds;

public record GetFilesByIdsResponse(
    IEnumerable<GetFileDto> Items
);