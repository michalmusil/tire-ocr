using Newtonsoft.Json;

namespace TireOcr.Ocr.Infrastructure.Dtos.GoogleGeminiResponse;

public record GoogleGeminiResponseDto(
    [property: JsonProperty("candidates")] List<ContentCandidate> ContentCandidates,
    [property: JsonProperty("usageMetadata")]
    UsageMetadata UsageMetadata
);