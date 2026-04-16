namespace AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.Auth.CreateApiKey;

public record CreateApiKeyRequest(
    string Name,
    DateTime? ValidUntil
);