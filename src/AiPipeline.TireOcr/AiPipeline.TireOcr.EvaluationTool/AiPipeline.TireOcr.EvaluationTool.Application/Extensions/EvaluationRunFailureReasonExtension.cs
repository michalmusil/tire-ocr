using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.RunFailure;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Extensions;

public static class EvaluationRunFailureReasonExtension
{
    public static string GetDescription(this EvaluationRunFailureReason failureReason)
    {
        return failureReason switch
        {
            EvaluationRunFailureReason.Preprocessing => "Run failed during preprocessing",
            EvaluationRunFailureReason.Ocr => "Run failed during OCR",
            EvaluationRunFailureReason.Postprocessing => "Run failed during postprocessing",
            EvaluationRunFailureReason.Unexpected => "Run failed due to unexpected reason",
            _ => throw new ArgumentOutOfRangeException(nameof(failureReason), failureReason, null)
        };
    }
}