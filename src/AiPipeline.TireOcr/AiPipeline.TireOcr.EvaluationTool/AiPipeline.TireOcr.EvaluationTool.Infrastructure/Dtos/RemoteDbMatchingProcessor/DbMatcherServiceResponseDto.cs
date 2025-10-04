using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemoteDbMatchingProcessor;

public record DbMatcherServiceResponseDto(
    string? ManufacturerDbMatch,
    List<TireDbMatchResponseDto> OrderedTireCodeDbMatches,
    long DurationMs
)
{
    public DbMatchingResultValueObject ToDomain()
    {
        return new DbMatchingResultValueObject
        {
            Matches = OrderedTireCodeDbMatches
                .Select(x => x.ToDomain())
                .ToList(),
            ManufacturerMatch = ManufacturerDbMatch,
            DurationMs = DurationMs
        };
    }
}