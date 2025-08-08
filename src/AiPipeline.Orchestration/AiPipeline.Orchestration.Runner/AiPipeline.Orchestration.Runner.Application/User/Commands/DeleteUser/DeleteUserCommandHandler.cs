using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.DeleteUser;

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}