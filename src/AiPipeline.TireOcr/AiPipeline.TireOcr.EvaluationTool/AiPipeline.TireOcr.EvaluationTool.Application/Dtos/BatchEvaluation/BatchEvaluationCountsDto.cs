namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos.BatchEvaluation;

public record BatchEvaluationCountsDto(
    int TotalCount,
    int FullyCorrectCount,
    int CorrectMainParametersCount,
    int FailedPreprocessingCount,
    int FailedOcrCount,
    int FailedPostprocessingCount,
    int FailedUnexpectedCount
);