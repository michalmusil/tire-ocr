namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;

public record BatchEvaluationCountsDto(
    int TotalCount,
    int FullyCorrectCount,
    int CorrectMainParametersCount,
    int InsufficientExtractionCount,
    int FalsePositiveCount,
    int FailedPreprocessingCount,
    int FailedOcrCount,
    int FailedPostprocessingCount,
    int FailedUnexpectedCount
);