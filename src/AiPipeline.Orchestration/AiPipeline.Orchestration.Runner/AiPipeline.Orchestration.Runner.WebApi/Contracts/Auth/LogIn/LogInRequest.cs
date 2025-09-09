namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Auth.LogIn;

public record LogInRequest(
    string Username,
    string Password
);