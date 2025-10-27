using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Services;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.LogIn;

public class LogInCommandHandler : ICommandHandler<LogInCommand, AuthenticatedUserDto>
{
    private const string UnauthorizedMessage = "Invalid credentials";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;
    private readonly IHashService _hashService;

    public LogInCommandHandler(IUnitOfWork unitOfWork, IAuthService authService, IHashService hashService)
    {
        _unitOfWork = unitOfWork;
        _authService = authService;
        _hashService = hashService;
    }

    public async Task<DataResult<AuthenticatedUserDto>> Handle(LogInCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await _unitOfWork.UserRepository.GetByUsernameAsync(request.Username);
        if (existingUser == null)
            return DataResult<AuthenticatedUserDto>.Unauthorized(UnauthorizedMessage);

        var passwordValid = _hashService.Verify(request.Password, existingUser.PasswordHash).IsSuccess;
        if (!passwordValid)
            return DataResult<AuthenticatedUserDto>.Unauthorized(UnauthorizedMessage);

        var tokensResult = await _authService.GetTokensForUserAsync(existingUser);
        if (tokensResult.IsFailure)
            return DataResult<AuthenticatedUserDto>.Unauthorized(UnauthorizedMessage);

        var tokens = tokensResult.Data!;
        var refreshTokenResult = existingUser.AddRefreshToken(tokens.RefreshToken, tokens.RefreshExpiration);
        if (refreshTokenResult.IsFailure)
            return DataResult<AuthenticatedUserDto>.Failure(new Failure(500, "Failed to log user in"));

        await _unitOfWork.SaveChangesAsync();
        var dto = AuthenticatedUserDto.FromDomain(existingUser, tokens);
        return DataResult<AuthenticatedUserDto>.Success(dto);
    }
}