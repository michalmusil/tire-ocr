using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.Auth.RefreshToken;

public record RefreshTokenResponse(
    AuthenticatedUserDto LoggedInUser
);