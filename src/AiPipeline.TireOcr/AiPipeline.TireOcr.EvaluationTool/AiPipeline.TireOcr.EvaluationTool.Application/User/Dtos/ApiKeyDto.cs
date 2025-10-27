using AiPipeline.TireOcr.EvaluationTool.Domain.UserAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;

public record ApiKeyDto(
    string Name,
    DateTime? ValidUntil,
    DateTime CreatedAt,
    DateTime UpdatedAt
)
{
    public static ApiKeyDto FromDomain(ApiKey domain)
    {
        return new ApiKeyDto(
            Name: domain.Name,
            ValidUntil: domain.ValidUntil,
            CreatedAt: domain.CreatedAt,
            UpdatedAt: domain.UpdatedAt
        );
    }
}