namespace AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.Auth.Register;

public record RegisterUserRequest
{
    public Guid? Id { get; init; }
    public required string Username { get; init; }
    public required string Password { get; set; }
}