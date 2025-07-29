namespace AiPipeline.Orchestration.Shared.Nodes.Dtos.GetFileUploadResult;

public record FileDetailsDto(
    Guid Id,
    string Path,
    string FileStorageScope,
    string StorageProvider,
    string ContentType
);