namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;

public record EvaluationRunFailureDto(
    string FailureReason,
    int Code,
    string Message
);