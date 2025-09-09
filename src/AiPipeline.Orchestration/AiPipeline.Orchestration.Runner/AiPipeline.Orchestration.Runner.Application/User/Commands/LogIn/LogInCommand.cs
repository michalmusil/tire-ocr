using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.LogIn;

public record LogInCommand(string Username, string Password) : ICommand<AuthenticatedUserDto>;