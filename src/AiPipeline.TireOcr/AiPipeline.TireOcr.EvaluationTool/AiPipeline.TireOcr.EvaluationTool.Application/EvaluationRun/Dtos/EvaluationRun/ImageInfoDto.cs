using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;

public record ImageInfoDto(
    string FileName,
    string ContentType
)
{
    public static ImageInfoDto FromDomain(ImageValueObject domain) => new (
        FileName: domain.FileName,
        ContentType: domain.ContentType
    );
}