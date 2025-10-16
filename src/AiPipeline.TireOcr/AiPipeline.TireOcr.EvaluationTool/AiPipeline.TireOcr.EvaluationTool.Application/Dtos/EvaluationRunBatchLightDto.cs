using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos;

public record EvaluationRunBatchLightDto(
    string Id,
    string Title,
    int NumberOfEvaluations,
    DateTime CreatedAt,
    DateTime? StartedAt,
    DateTime? FinishedAt
)
{
    public static EvaluationRunBatchLightDto FromDomain(EvaluationRunBatchEntity domain)
    {
        return new EvaluationRunBatchLightDto(
            Id: domain.Id.ToString(),
            Title: domain.Title,
            NumberOfEvaluations: domain.EvaluationRuns.Count,
            CreatedAt: domain.CreatedAt,
            StartedAt: domain.StartedAt,
            FinishedAt: domain.FinishedAt
        );
    }
}