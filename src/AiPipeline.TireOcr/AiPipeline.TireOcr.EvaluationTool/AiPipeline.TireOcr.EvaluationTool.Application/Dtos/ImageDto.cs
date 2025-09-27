using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos;

public record ImageDto(
    string Name,
    string ContentType,
    byte[] ImageData
)
{
    public ImageValueObject ToDomain() => new()
    {
        FileName = Name,
        ContentType = ContentType
    };
}