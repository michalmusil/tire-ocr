using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid UpdatingUserId,
    Guid UserToUpdateId,
    string? Username,
    string? Password
) : ICommand<UserDto>;