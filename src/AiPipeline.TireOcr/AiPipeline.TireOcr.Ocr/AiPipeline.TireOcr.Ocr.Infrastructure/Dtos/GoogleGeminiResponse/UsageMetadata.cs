using Newtonsoft.Json;

namespace TireOcr.Ocr.Infrastructure.Dtos.GoogleGeminiResponse;

public record UsageMetadata(
    [property: JsonProperty("promptTokenCount")] int PromptTokenCount,
    [property: JsonProperty("candidatesTokenCount")] int CandidatesTokenCount,
    [property: JsonProperty("totalTokenCount")] int TotalTokenCount
);
