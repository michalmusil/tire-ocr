using Newtonsoft.Json;

namespace TireOcr.Ocr.Infrastructure.Dtos.MistralResponse;

public record Message(
    [property: JsonProperty("role")] string Role,
    [property: JsonProperty("content")] string Content
);
