namespace AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.Auth.RefreshToken;

public record RefreshTokenRequest(
    string RefreshToken,
    string AccessToken
);