namespace AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;

public record FileReference(
    Guid Id,
    string StorageProvider,
    string Path,
    string ContentType
);