using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.AddApiKey;

public record AddApiKeyCommand(Guid UserId, Guid CreatorUserId, string Name, DateTime? ValidUntil)
    : ICommand<NewApiKeyDto>;