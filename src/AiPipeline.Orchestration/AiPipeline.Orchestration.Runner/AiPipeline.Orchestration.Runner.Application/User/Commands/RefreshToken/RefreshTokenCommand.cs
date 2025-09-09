using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<AuthenticatedUserDto>;