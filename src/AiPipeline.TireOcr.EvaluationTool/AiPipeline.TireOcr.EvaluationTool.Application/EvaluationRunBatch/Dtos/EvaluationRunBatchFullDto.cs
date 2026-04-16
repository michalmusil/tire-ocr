using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos;

public record EvaluationRunBatchFullDto(
    string Id,
    string Title,
    string? Description,
    DateTime? StartedAt,
    DateTime? FinishedAt,
    RunConfigDto? BatchConfig,
    BatchEvaluationDto? BatchEvaluation,
    IEnumerable<EvaluationRunDto> EvaluationRuns
)
{
    public static EvaluationRunBatchFullDto FromDomain(EvaluationRunBatchEntity domain,
        BatchEvaluationDto? batchEvaluation)
    {
        var firstEvaluationRun = domain.EvaluationRuns.FirstOrDefault();

        return new EvaluationRunBatchFullDto(
            Id: domain.Id.ToString(),
            Title: domain.Title,
            Description: domain.Description,
            StartedAt: domain.StartedAt,
            FinishedAt: domain.FinishedAt,
            BatchConfig: firstEvaluationRun is null
                ? null
                : new(
                    PreprocessingType: firstEvaluationRun.PreprocessingType,
                    OcrType: firstEvaluationRun.OcrType,
                    PostprocessingType: firstEvaluationRun.PostprocessingType,
                    DbMatchingType: firstEvaluationRun.DbMatchingType
                ),
            BatchEvaluation: batchEvaluation,
            EvaluationRuns: domain.EvaluationRuns
                .Select(EvaluationRunDto.FromDomain)
        );
    }
}