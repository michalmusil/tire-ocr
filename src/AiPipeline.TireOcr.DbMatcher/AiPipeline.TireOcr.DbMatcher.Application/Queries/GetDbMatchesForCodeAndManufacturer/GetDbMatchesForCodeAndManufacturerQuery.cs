using AiPipeline.TireOcr.DbMatcher.Application.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.DbMatcher.Application.Queries.GetDbMatchesForCodeAndManufacturer;

public record GetDbMatchesForCodeAndManufacturerQuery(
    DetectedTireCodeDto DetectedCode,
    string? DetectedManufacturer,
    int? MaxEntries
) : IQuery<DbMatchingResultDto>;