using Newtonsoft.Json;

namespace TireOcr.Ocr.Infrastructure.Dtos.MistralResponse;

public record MistralResponseDto(
    [property: JsonProperty("id")] string Id,
    [property: JsonProperty("object")] string Object,
    [property: JsonProperty("created")] long Created,
    [property: JsonProperty("model")] string Model,
    [property: JsonProperty("choices")] List<Choice> Choices,
    [property: JsonProperty("usage")] Usage Usage
);