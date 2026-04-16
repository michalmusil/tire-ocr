using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.LogIn;

public record LogInCommand(string Username, string Password) : ICommand<AuthenticatedUserDto>;