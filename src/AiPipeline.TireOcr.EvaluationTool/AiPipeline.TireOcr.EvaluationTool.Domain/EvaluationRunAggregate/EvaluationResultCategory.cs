namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public enum EvaluationResultCategory
{
    // No evaluation was performed as user didn't provide expected code
    NoEvaluation,

    // All parameters specified in the expected tire code were extracted and match fully.
    FullyCorrect,

    // At least Width, Diameter, Aspect Ratio and Construction were extracted and match fully with expected values.
    CorrectInMainParameters,

    // No code was detected on the Preprocessing step
    NoCodeDetectedPreprocessing,

    // No code was detected on the OCR step
    NoCodeDetectedOcr,

    // No code was detected on the Postprocessing step
    NoCodeDetectedPostprocessing,

    // No code was detected for unexpected reason
    NoCodeDetectedUnexpected,

    // At least one of Width, Diameter, Aspect Ratio and Construction were not extracted
    InsufficientExtraction,

    // At least one of the extracted parameters didn't match expected tire code
    FalsePositive,
}