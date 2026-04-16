using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Services;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.UpdateUser;

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
        if (request.UpdatingUserId != request.UserToUpdateId)
            return DataResult<UserDto>.Forbidden("Users can only update their own account");

        var userToUpdate = await _unitOfWork.UserRepository.GetByIdAsync(request.UserToUpdateId);
        if (userToUpdate is null)
            return DataResult<UserDto>.NotFound($"User with id {request.UserToUpdateId} not found.");

        if (request.Username is not null)
        {
            var otherUserWithUsername = await _unitOfWork.UserRepository.GetByUsernameAsync(request.Username);
            if (otherUserWithUsername is not null && otherUserWithUsername.Id != request.UserToUpdateId)
                return DataResult<UserDto>.Conflict("User with this username already exists");
        }

        string? newPasswordHash = null;
        if (request.Password != null)
        {
            var hashResult = _hashService.GetHashOf(request.Password);
            if (hashResult.IsFailure)
                return DataResult<UserDto>.Failure(hashResult.Failures);
            newPasswordHash = hashResult.Data;
        }


        var updateResults = new List<Result>
        {
            userToUpdate.SetUsername(request.Username!),
        };
        if (newPasswordHash != null)
            updateResults.Add(userToUpdate.SetPasswordHash(newPasswordHash));

        var failures = updateResults
            .SelectMany(r => r.Failures)
            .ToArray();

        if (failures.Any())
            return DataResult<UserDto>.Failure(failures);

        await _unitOfWork.SaveChangesAsync();
        var dto = UserDto.FromDomain(userToUpdate);

        return DataResult<UserDto>.Success(dto);
    }
}