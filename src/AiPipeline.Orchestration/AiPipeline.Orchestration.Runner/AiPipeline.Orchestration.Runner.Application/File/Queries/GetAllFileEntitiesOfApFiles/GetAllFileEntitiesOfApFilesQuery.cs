using AiPipeline.Orchestration.Shared.Contracts.Schema.Properties;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Queries.GetAllFileEntitiesOfApFiles;

public record GetAllFileEntitiesOfApFilesQuery(IEnumerable<ApFile> ApFiles)
    : IQuery<Dictionary<ApFile, Domain.FileAggregate.File>>;