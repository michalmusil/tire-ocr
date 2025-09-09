using AiPipeline.Orchestration.Runner.Domain.UserAggregate;

namespace AiPipeline.Orchestration.Runner.Application.User.Dtos;

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