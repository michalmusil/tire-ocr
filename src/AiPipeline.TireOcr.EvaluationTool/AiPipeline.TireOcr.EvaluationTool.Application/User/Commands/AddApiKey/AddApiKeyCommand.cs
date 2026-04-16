using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.AddApiKey;

public record AddApiKeyCommand(Guid UserId, Guid CreatorUserId, string Name, DateTime? ValidUntil)
    : ICommand<NewApiKeyDto>;