namespace AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;

public enum OcrType
{
    GoogleGemini = 0,
    MistralPixtral = 1,
    OpenAiGpt = 2,
    GoogleCloudVision = 3,
    AzureAiVision = 4,
    PaddleOcr = 5,
    QwenVl = 6,
    InternVl = 7,
    EasyOcr = 8,
    DeepseekOcr = 9
}