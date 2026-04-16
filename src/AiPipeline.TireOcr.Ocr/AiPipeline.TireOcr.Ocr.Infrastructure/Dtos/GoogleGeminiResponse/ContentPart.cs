using Newtonsoft.Json;

namespace TireOcr.Ocr.Infrastructure.Dtos.GoogleGeminiResponse;

public record ContentPart(
    [property: JsonProperty("text")] string Text
);