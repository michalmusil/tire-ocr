namespace AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;

public enum PreprocessingType
{
    Resize = 0,
    ExtractRoi = 1,
    ExtractRoiAndRemoveBg = 2,
    ExtractAndComposeSlices = 3,
}