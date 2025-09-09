namespace AiPipeline.Orchestration.Runner.Application.User.Dtos;

public record AccessRefreshTokenPair(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshExpiration
);