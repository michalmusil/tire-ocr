using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.Auth.CreateApiKey;

public record CreateApiKeyResponse(
    NewApiKeyDto ApiKey
);