namespace AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetFileById;

public record GetFileByIdRequest(Guid Id, Guid UserId);