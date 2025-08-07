using AiPipeline.Orchestration.FileService.Domain.FileAggregate;

namespace AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetAllFiles;

public record GetAllFilesRequest(
    FileStorageScope? StorageScopeFilter,
    int PageNumber,
    int PageSize
);