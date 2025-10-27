using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Common.Dtos;

public record ImageDto(
    string FileName,
    string ContentType,
    byte[] ImageData
)
{
    public ImageValueObject ToDomain() => new()
    {
        FileName = FileName,
        ContentType = ContentType
    };
}