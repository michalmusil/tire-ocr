namespace AiPipeline.Orchestration.Shared.Nodes.Dtos.FileReferenceUploader;

public record UploadFileParams(
    Guid FileId,
    Guid UserId,
    Stream FileStream,
    string ContentType,
    string FileName,
    bool StorePermanently
);