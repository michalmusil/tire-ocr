namespace AiPipeline.TireOcr.Shared.Models;

public enum TireCodeDetectorType
{
    GoogleGemini = 0,
    MistralPixtral = 1,
    OpenAiGpt = 2,
    GoogleCloudVision = 3,
    AzureAiVision = 4,
    QwenVl = 5,
    InternVl = 6,
    DeepseekOcr = 7,
    HunyuanOcr = 8,
}