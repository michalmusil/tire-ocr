using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid UpdatingUserId,
    Guid UserToUpdateId,
    string? Username,
    string? Password
) : ICommand<UserDto>;