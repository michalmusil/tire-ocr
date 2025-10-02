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
                .Select(x => new TireDbMatch(
                    RequiredCharEdits: x.RequiredCharEdits,
                    EstimatedAccuracy: x.EstimatedAccuracy,
                    Width: x.TireEntry.Width,
                    Diameter: x.TireEntry.Diameter,
                    Profile: x.TireEntry.Profile,
                    Construction: x.TireEntry.Construction,
                    LoadIndex: x.TireEntry.LoadIndex,
                    SpeedIndex: x.TireEntry.SpeedIndex,
                    LoadIndexSpeedIndex: x.TireEntry.LoadIndexSpeedIndex
                )).ToList(),
            ManufacturerMatch = ManufacturerDbMatch,
            DurationMs = DurationMs
        };
    }
}