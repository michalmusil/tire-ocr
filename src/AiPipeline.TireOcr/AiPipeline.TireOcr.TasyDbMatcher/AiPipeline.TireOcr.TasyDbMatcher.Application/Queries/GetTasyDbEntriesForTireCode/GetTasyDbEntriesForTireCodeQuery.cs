using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Queries.GetTasyDbEntriesForTireCode;

public record GetTasyDbEntriesForTireCodeQuery(DetectedTireCodeDto DetectedCode)
    : IQuery<List<ProcessedTireParamsDatabaseEntryDto>>;