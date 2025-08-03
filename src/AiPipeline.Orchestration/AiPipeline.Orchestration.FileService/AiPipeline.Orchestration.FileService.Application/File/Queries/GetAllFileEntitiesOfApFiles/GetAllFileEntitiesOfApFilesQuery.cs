using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.FileService.Application.File.Queries.GetAllFileEntitiesOfApFiles;

public record GetAllFileEntitiesOfApFilesQuery(IEnumerable<ApFile> ApFiles)
    : IQuery<Dictionary<ApFile, Domain.FileAggregate.File>>;