using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using AiPipeline.Orchestration.Runner.Application.User.Services;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.UpdateUser;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHashService _hashService;


    public UpdateUserCommandHandler(IUnitOfWork unitOfWork, IHashService hashService)
    {
        _unitOfWork = unitOfWork;
        _hashService = hashService;
    }

    public async Task<DataResult<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}