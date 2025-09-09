namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Auth.CreateApiKey;

public record CreateApiKeyRequest(
    string Name,
    DateTime? ValidUntil
);