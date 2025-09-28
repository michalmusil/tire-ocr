using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemoteDbMatchingProcessor;

public record DbMatcherServiceResponseDto(
    List<TireDbMatchResponseDto> OrderedTireCodeDbMatches,
    string? ManufacturerDbMatch
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
                    Width: x.TireEntryResponse.Width,
                    Diameter: x.TireEntryResponse.Diameter,
                    Profile: x.TireEntryResponse.Profile,
                    Construction: x.TireEntryResponse.Construction,
                    LoadIndex: x.TireEntryResponse.LoadIndex,
                    SpeedIndex: x.TireEntryResponse.SpeedIndex,
                    LoadIndexSpeedIndex: x.TireEntryResponse.LoadIndexSpeedIndex
                )).ToList(),
            ManufacturerMatch = ManufacturerDbMatch,
            DurationMs = 0 // TODO: add to remote dbMatching service
        };
    }
}