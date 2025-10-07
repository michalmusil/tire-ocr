using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos;

public record EvaluationRunBatchLightDto(
    string Id,
    string Title
)
{
    public static EvaluationRunBatchLightDto FromDomain(EvaluationRunBatchEntity domain)
    {
        return new EvaluationRunBatchLightDto(
            Id: domain.Id.ToString(),
            Title: domain.Title
        );
    }
}