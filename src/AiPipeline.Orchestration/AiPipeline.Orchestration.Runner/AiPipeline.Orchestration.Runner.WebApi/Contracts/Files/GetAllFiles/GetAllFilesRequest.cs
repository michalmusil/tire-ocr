using AiPipeline.Orchestration.Runner.Domain.FileAggregate;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Files.GetAllFiles;

public record GetAllFilesRequest(
    FileStorageScope? StorageScopeFilter,
    int PageNumber,
    int PageSize
);