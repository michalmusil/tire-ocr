namespace AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;

public enum PreprocessingType
{
    Resize = 0,
    ExtractRoi = 1,
    ExtractRoiAndEnhanceCharacters = 2,
    ExtractAndComposeSlices = 3,
    ExtractAndComposeSlicesEnhanceCharacters = 4,
}