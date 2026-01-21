namespace TireOcr.Ocr.Infrastructure.Dtos.DeepseekOcrResponse;

public record Message(
    string Content,
    string Role
);