using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using AiPipeline.Orchestration.Runner.Application.User.Services;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.RegisterUser;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHashService _hashService;


    public RegisterUserCommandHandler(IUnitOfWork unitOfWork, IHashService hashService)
    {
        _unitOfWork = unitOfWork;
        _hashService = hashService;
    }

    public async Task<DataResult<UserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}