namespace TireOcr.RunnerPrototype.Models;

public enum TireCodeDetectorType
{
    GoogleGemini = 0,
    MistralPixtral = 1,
    OpenAiGpt = 2,
    GoogleCloudVision = 3,
    AzureAiVision = 4,
    Ollama = 5,
    LmStudio = 6
}