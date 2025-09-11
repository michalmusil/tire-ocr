namespace AiPipeline.Orchestration.Shared.NodeSdk.Dtos.FileReferenceUploader;

public record UploadFileParams(
    Guid FileId,
    Guid UserId,
    Stream FileStream,
    string ContentType,
    string FileName,
    bool StorePermanently
);