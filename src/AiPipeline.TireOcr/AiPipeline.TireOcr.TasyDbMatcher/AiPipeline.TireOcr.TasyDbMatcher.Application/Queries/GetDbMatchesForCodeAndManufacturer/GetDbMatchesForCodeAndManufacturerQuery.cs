using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Queries.GetDbMatchesForCodeAndManufacturer;

public record GetDbMatchesForCodeAndManufacturerQuery(
    DetectedTireCodeDto DetectedCode,
    string? DetectedManufacturer,
    int? MaxEntries
) : IQuery<DbMatchingResultDto>;