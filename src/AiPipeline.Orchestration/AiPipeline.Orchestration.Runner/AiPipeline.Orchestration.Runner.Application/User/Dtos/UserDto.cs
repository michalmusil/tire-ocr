namespace AiPipeline.Orchestration.Runner.Application.User.Dtos;

public record UserDto(
    string Id,
    string Username,
    DateTime CreatedAt,
    DateTime UpdatedAt
);