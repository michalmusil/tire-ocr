namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.RunFailure;

public enum EvaluationRunFailureReason
{
    Preprocessing,
    Ocr,
    Postprocessing,
    Unexpected
}