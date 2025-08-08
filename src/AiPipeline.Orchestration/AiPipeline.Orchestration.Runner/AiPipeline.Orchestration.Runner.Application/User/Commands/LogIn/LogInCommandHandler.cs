using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using AiPipeline.Orchestration.Runner.Application.User.Services;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.LogIn;

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
        throw new NotImplementedException();
    }
}