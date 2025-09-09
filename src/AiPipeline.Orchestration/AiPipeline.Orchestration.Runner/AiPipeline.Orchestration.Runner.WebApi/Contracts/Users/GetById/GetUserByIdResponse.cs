using AiPipeline.Orchestration.Runner.Application.User.Dtos;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Users.GetById;

public record GetUserByIdResponse(
    UserDto User
);