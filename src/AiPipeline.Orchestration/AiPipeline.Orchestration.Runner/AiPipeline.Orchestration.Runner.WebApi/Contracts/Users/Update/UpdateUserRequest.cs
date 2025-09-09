namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Users.Update;

public record UpdateUserRequest(
    string? Username,
    string? Password
);