namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public enum EvaluationRunFailureReason
{
    Preprocessing,
    Ocr,
    Postprocessing,
    Unexpected
}