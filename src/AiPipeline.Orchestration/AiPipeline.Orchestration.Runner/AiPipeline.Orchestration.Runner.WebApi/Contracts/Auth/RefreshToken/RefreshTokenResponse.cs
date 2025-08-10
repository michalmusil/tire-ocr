using AiPipeline.Orchestration.Runner.Application.User.Dtos;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Auth.RefreshToken;

public record RefreshTokenResponse(
    AuthenticatedUserDto LoggedInUser
);