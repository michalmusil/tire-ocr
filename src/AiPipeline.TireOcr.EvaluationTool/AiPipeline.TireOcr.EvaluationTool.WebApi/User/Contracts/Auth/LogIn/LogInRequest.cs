namespace AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.Auth.LogIn;

public record LogInRequest(
    string Username,
    string Password
);