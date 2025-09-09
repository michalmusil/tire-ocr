using AiPipeline.Orchestration.FileService.Domain.FileAggregate;

namespace AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetAllFiles;

public record GetAllFilesRequest(
    FileStorageScope? StorageScopeFilter,
    Guid UserId,
    int PageNumber,
    int PageSize
);