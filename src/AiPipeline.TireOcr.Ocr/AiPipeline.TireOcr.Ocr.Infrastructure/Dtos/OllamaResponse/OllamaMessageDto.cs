using System.Text.Json.Serialization;

namespace TireOcr.Ocr.Infrastructure.Dtos.OllamaResponse;

public record OllamaMessageDto(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")]
    string Content
);