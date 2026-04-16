namespace AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.User.Update;

public record UpdateUserRequest(
    string? Username,
    string? Password
);