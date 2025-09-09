namespace AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetFilesByIds;

public record GetFilesByIdsRequest(IEnumerable<Guid> Ids, Guid UserId, bool FailIfNotAllFound);