using AiPipeline.Orchestration.Runner.Application.User.Dtos;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Auth.LogIn;

public record LogInResponse(
    AuthenticatedUserDto LoggedInUser
);