using AiPipeline.Orchestration.Runner.Application.User.Dtos;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Auth.Register;

public record RegisterUserResponse(
    UserDto User
);