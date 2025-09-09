namespace AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.DownloadFile;

public record DownloadFileResponse(string ContentType, Stream FileData);