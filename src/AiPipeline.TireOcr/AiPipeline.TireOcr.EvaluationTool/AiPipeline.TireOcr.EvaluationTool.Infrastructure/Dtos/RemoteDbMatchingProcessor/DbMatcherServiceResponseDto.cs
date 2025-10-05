using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemoteDbMatchingProcessor;

public record DbMatcherServiceResponseDto(
    string? ManufacturerDbMatch,
    List<TireDbMatchResponseDto> OrderedTireCodeDbMatches,
    long DurationMs
)
{
    public DbMatchingResultEntity ToDomain()
    {
        return new DbMatchingResultEntity
        (
            matches: OrderedTireCodeDbMatches
                .Select(x => x.ToDomain())
                .ToList(),
            manufacturerMatch: ManufacturerDbMatch,
            durationMs: DurationMs
        );
    }
}