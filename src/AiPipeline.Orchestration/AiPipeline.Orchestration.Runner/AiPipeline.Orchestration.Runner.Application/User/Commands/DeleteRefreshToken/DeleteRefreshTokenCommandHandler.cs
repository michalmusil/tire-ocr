using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.User.Services;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.DeleteRefreshToken;

public class DeleteRefreshTokenCommandHandler : ICommandHandler<DeleteRefreshTokenCommand>
{
    private readonly Result _unauthorizedFailure = Result.Unauthorized("Unauthorized to refresh");

    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;

    public DeleteRefreshTokenCommandHandler(IUnitOfWork unitOfWork, IAuthService authService)
    {
        _unitOfWork = unitOfWork;
        _authService = authService;
    }

    public async Task<Result> Handle(DeleteRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}