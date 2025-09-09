using AiPipeline.Orchestration.Runner.Application.User.Dtos;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Users.Update;

public record UpdateUserResponse(
    UserDto User
);