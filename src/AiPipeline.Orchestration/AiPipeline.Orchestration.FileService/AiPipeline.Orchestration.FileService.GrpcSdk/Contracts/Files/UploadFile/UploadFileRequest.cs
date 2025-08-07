namespace AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.UploadFile;

public record UploadFileRequest(
    string FileName,
    string ContentType,
    Stream FileData,
    Guid? Id
);