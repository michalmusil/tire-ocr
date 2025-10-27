using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.RegisterUser;

public record RegisterUserCommand(Guid? Id, string Username, string Password) : ICommand<UserDto>;