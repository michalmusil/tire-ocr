using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;

public record EvaluationRunBatchDto(
    string Id,
    DateTime StartedAt,
    DateTime FinishedAt,
    IEnumerable<EvaluationRunDto> EvaluationRuns
)
{
    public static EvaluationRunBatchDto FromDomain(EvaluationRunBatch domain)
    {
        return new EvaluationRunBatchDto(
            Id: domain.Id.ToString(),
            StartedAt: domain.StartedAt ?? DateTime.MinValue.ToUniversalTime(),
            FinishedAt: domain.FinishedAt ?? DateTime.UtcNow,
            EvaluationRuns: domain.EvaluationRuns
                .Select(EvaluationRunDto.FromDomain)
        );
    }
}