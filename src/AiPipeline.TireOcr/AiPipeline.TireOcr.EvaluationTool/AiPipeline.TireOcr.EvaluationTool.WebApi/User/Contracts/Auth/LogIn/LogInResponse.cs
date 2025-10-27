using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.Auth.LogIn;

public record LogInResponse(
    AuthenticatedUserDto LoggedInUser
);