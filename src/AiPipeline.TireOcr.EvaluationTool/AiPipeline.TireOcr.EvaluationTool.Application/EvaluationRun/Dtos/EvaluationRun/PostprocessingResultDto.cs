using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;

public record PostprocessingResultDto(
    TireCodeDto PostprocessedTireCode,
    long DurationMs
)
{
    public static PostprocessingResultDto FromDomain(PostprocessingResultEntity domain) => new(
        PostprocessedTireCode: TireCodeDto.FromDomain(domain.TireCode),
        DurationMs: domain.DurationMs);
}