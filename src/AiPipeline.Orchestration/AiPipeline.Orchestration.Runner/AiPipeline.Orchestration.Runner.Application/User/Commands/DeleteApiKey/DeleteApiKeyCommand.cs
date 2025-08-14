using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.DeleteApiKey;

public record DeleteApiKeyCommand(Guid UserId, Guid DeletingUserId, string Name)
    : ICommand<UserDto>;