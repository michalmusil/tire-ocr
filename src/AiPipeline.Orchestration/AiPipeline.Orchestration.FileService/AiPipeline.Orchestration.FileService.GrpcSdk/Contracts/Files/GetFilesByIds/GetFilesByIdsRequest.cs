namespace AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetFilesByIds;

public record GetFilesByIdsRequest(IEnumerable<Guid> Ids, bool FailIfNotAllFound);