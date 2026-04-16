using System.Text.Json.Serialization;

namespace TireOcr.Ocr.Infrastructure.Dtos.OllamaResponse;

public record OllamaResponseDto(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("created_at")]
    DateTime CreatedAt,
    [property: JsonPropertyName("message")]
    OllamaMessageDto Message,
    [property: JsonPropertyName("done")] bool Done,
    [property: JsonPropertyName("done_reason")]
    string DoneReason,
    [property: JsonPropertyName("total_duration")]
    long TotalDuration,
    [property: JsonPropertyName("load_duration")]
    long LoadDuration,
    [property: JsonPropertyName("prompt_eval_count")]
    int PromptEvalCount,
    [property: JsonPropertyName("prompt_eval_duration")]
    long PromptEvalDuration,
    [property: JsonPropertyName("eval_count")]
    int EvalCount,
    [property: JsonPropertyName("eval_duration")]
    long EvalDuration
);