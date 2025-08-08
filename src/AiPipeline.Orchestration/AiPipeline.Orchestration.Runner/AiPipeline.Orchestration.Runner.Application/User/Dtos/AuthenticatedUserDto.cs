namespace AiPipeline.Orchestration.Runner.Application.User.Dtos;

public record AuthenticatedUserDto(
    string Id,
    string Username,
    AccessRefreshTokenPair AccessRefreshTokenPair
);