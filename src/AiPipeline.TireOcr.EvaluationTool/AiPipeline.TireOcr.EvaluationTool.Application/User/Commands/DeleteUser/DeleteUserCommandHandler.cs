using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.DeleteUser;

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        if (request.UserToDeleteId != request.DeletingUserId)
            return Result.Forbidden("Users may only delete their own account");

        var userToDelete = await _unitOfWork.UserRepository.GetByIdAsync(request.UserToDeleteId);
        if (userToDelete is null)
            return Result.NotFound($"User with id {request.UserToDeleteId} not found");

        await _unitOfWork.UserRepository.RemoveAsync(userToDelete);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}