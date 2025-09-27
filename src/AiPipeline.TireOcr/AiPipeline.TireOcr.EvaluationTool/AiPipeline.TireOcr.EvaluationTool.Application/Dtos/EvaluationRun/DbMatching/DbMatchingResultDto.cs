using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun.DbMatching;

public record DbMatchingResultDto(
    List<TireCodeMatchDto> Matches,
    string? ManufacturerMatch,
    long DurationMs
)
{
    public static DbMatchingResultDto FromDomain(DbMatchingResultValueObject domain)
    {
        return new DbMatchingResultDto(
            Matches: domain.Matches
                .Select(m => new TireCodeMatchDto(
                    RequiredCharEdits: m.RequiredCharEdits,
                    EstimatedAccuracy: m.EstimatedAccuracy,
                    Width: m.Width,
                    Diameter: m.Diameter,
                    Profile: m.Profile,
                    Construction: m.Construction,
                    LoadIndexSpeedIndex: m.LoadIndexSpeedIndex,
                    LoadIndex: m.LoadIndex,
                    SpeedIndex: m.SpeedIndex
                )).ToList(),
            ManufacturerMatch: domain.ManufacturerMatch,
            DurationMs: domain.DurationMs
        );
    }
}