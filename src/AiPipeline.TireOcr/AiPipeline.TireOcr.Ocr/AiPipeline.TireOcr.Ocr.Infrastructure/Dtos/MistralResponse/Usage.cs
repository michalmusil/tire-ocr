using Newtonsoft.Json;

namespace TireOcr.Ocr.Infrastructure.Dtos.MistralResponse;

public record Usage(
    [property: JsonProperty("prompt_tokens")] int PromptTokens,
    [property: JsonProperty("total_tokens")] int TotalTokens,
    [property: JsonProperty("completion_tokens")] int CompletionTokens
);