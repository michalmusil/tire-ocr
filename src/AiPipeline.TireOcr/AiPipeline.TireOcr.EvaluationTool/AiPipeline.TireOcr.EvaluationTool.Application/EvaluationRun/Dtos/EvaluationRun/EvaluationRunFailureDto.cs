namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;

public record EvaluationRunFailureDto(
    string FailureReason,
    int Code,
    string Message
);