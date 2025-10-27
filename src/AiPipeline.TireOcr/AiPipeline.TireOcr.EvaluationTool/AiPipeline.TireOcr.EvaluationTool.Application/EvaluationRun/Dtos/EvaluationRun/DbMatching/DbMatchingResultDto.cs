using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun.DbMatching;

public record DbMatchingResultDto(
    List<TireCodeMatchDto> Matches,
    string? ManufacturerMatch,
    long DurationMs
)
{
    public static DbMatchingResultDto FromDomain(DbMatchingResultEntity domain)
    {
        return new DbMatchingResultDto(
            Matches: domain.Matches
                .Select(TireCodeMatchDto.FromDomain)
                .ToList(),
            ManufacturerMatch: domain.ManufacturerMatch,
            DurationMs: domain.DurationMs
        );
    }
}