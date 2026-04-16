using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<AuthenticatedUserDto>;