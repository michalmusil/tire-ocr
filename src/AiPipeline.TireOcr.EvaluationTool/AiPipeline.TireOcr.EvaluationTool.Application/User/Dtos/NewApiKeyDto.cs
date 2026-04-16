using AiPipeline.TireOcr.EvaluationTool.Domain.UserAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;

public record NewApiKeyDto(
    string Name,
    string Key,
    DateTime? ValidUntil
)
{
    public static NewApiKeyDto FromDomain(ApiKey domain)
    {
        return new NewApiKeyDto(domain.Name, domain.Key, domain.ValidUntil);
    }
}