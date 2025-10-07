using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos;

public record EvaluationRunBatchFullDto(
    string Id,
    string Title,
    DateTime StartedAt,
    DateTime FinishedAt,
    IEnumerable<EvaluationRunDto> EvaluationRuns
)
{
    public static EvaluationRunBatchFullDto FromDomain(EvaluationRunBatchEntity domain)
    {
        return new EvaluationRunBatchFullDto(
            Id: domain.Id.ToString(),
            Title: domain.Title,
            StartedAt: domain.StartedAt ?? DateTime.MinValue.ToUniversalTime(),
            FinishedAt: domain.FinishedAt ?? DateTime.UtcNow,
            EvaluationRuns: domain.EvaluationRuns
                .Select(EvaluationRunDto.FromDomain)
        );
    }
}