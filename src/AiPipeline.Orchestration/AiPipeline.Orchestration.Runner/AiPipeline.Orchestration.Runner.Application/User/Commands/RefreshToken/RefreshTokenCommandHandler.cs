using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using AiPipeline.Orchestration.Runner.Application.User.Services;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.RefreshToken;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthenticatedUserDto>
{
    private readonly DataResult<AuthenticatedUserDto> _unauthorizedFailure =
        DataResult<AuthenticatedUserDto>.Unauthorized("Unauthorized to refresh");

    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, IAuthService authService)
    {
        _unitOfWork = unitOfWork;
        _authService = authService;
    }

    public async Task<DataResult<AuthenticatedUserDto>> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}