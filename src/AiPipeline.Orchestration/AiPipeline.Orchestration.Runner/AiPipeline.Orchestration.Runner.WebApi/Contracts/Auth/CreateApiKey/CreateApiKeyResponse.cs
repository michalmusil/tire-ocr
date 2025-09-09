using AiPipeline.Orchestration.Runner.Application.User.Dtos;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Auth.CreateApiKey;

public record CreateApiKeyResponse(
    NewApiKeyDto ApiKey
);