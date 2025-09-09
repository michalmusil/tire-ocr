using Newtonsoft.Json;

namespace TireOcr.Ocr.Infrastructure.Dtos.GoogleGeminiResponse;

public record ContentCandidate(
    [property: JsonProperty("content")] Content Content
);
