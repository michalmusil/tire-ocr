using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.RegisterUser;

public record RegisterUserCommand(Guid? Id, string Username, string Password) : ICommand<UserDto>;