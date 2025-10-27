using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos;

public record EvaluationRunBatchFullDto(
    string Id,
    string Title,
    DateTime? StartedAt,
    DateTime? FinishedAt,
    BatchEvaluationDto? BatchEvaluation,
    IEnumerable<EvaluationRunDto> EvaluationRuns
)
{
    public static EvaluationRunBatchFullDto FromDomain(EvaluationRunBatchEntity domain,
        BatchEvaluationDto? batchEvaluation)
    {
        return new EvaluationRunBatchFullDto(
            Id: domain.Id.ToString(),
            Title: domain.Title,
            StartedAt: domain.StartedAt,
            FinishedAt: domain.FinishedAt,
            BatchEvaluation: batchEvaluation,
            EvaluationRuns: domain.EvaluationRuns
                .Select(EvaluationRunDto.FromDomain)
        );
    }
}