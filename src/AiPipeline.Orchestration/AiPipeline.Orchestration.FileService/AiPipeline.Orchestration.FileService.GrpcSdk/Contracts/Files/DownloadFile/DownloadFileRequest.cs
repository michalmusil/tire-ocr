namespace AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.DownloadFile;

public record DownloadFileRequest(Guid Id, Guid UserId);