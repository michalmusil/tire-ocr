namespace TireOcr.Ocr.Infrastructure.Dtos.DeepseekOcrResponse;

public record Output(
    List<Choice> Choices
);