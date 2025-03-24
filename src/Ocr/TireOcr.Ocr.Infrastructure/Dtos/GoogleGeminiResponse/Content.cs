using Newtonsoft.Json;

namespace TireOcr.Ocr.Infrastructure.Dtos.GoogleGeminiResponse;

public record Content(
    [property: JsonProperty("parts")] List<ContentPart> Parts,
    [property: JsonProperty("role")] string Role
);