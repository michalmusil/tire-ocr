using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.DeleteApiKey;

public record DeleteApiKeyCommand(Guid UserId, Guid DeletingUserId, string Name)
    : ICommand<UserDto>;