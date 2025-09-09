using AiPipeline.Orchestration.FileService.Domain.FileAggregate;

namespace AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.UploadFile;

public record UploadFileRequest(
    string FileName,
    string ContentType,
    Stream FileData,
    Guid UserId,
    Guid? Id,
    FileStorageScope FileStorageScope = FileStorageScope.ShortTerm
);