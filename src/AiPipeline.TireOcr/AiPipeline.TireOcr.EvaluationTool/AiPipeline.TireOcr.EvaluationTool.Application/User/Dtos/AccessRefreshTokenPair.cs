namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;

public record AccessRefreshTokenPair(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshExpiration
);