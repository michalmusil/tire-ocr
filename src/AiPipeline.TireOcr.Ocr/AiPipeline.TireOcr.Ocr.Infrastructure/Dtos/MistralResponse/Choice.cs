using Newtonsoft.Json;

namespace TireOcr.Ocr.Infrastructure.Dtos.MistralResponse;

public record Choice(
    [property: JsonProperty("index")] int Index,
    [property: JsonProperty("message")] Message Message,
    [property: JsonProperty("finish_reason")] string FinishReason
);