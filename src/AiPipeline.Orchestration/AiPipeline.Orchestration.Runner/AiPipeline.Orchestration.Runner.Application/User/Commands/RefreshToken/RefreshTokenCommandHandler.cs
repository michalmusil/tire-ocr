using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using AiPipeline.Orchestration.Runner.Application.User.Services;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.RefreshToken;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthenticatedUserDto>
{
    private readonly DataResult<AuthenticatedUserDto> _unauthorizedFailure =
        DataResult<AuthenticatedUserDto>.Unauthorized("Unauthorized to refresh");

    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, IAuthService authService,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _authService = authService;
        _logger = logger;
    }

    public async Task<DataResult<AuthenticatedUserDto>> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var userResult = await _authService.GetUserFromAccessTokenAsync(request.AccessToken);
        if (userResult.IsFailure)
            return DataResult<AuthenticatedUserDto>.Failure(userResult.Failures);

        var foundUser = userResult.Data!;
        var foundValidRefreshToken = foundUser.RefreshTokens.FirstOrDefault(rt =>
            rt.Token == request.RefreshToken &&
            rt.ExpiresAt > DateTime.UtcNow
        );
        if (foundValidRefreshToken is null)
            return _unauthorizedFailure;

        if (foundValidRefreshToken.Invalidated)
        {
            _logger.LogWarning(
                $"Possible token breach: attempted multiple usage of '{foundValidRefreshToken.Token}' refresh token"
            );
            return _unauthorizedFailure;
        }

        var invalidateTokenResult = foundUser.InvalidateRefreshToken(foundValidRefreshToken.Token);
        if (invalidateTokenResult.IsFailure)
            return _unauthorizedFailure;

        var newTokensResult = await _authService.GetTokensForUserAsync(foundUser);
        if (newTokensResult.IsFailure)
            return _unauthorizedFailure;

        var tokens = newTokensResult.Data!;
        var addTokenResult = foundUser.AddRefreshToken(tokens.RefreshToken, tokens.RefreshExpiration);
        if (addTokenResult.IsFailure)
            return _unauthorizedFailure;

        await _unitOfWork.SaveChangesAsync();
        var dto = AuthenticatedUserDto.FromDomain(foundUser, tokens);
        return DataResult<AuthenticatedUserDto>.Success(dto);
    }
}