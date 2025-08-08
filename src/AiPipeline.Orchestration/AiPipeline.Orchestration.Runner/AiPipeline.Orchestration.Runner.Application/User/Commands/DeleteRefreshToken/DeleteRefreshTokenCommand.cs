using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.DeleteRefreshToken;

public record DeleteRefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand;