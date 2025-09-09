using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.DeleteApiKey;

public class DeleteApiKeyCommandHandler : ICommandHandler<DeleteApiKeyCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteApiKeyCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DataResult<UserDto>> Handle(DeleteApiKeyCommand request, CancellationToken cancellationToken)
    {
        if (request.DeletingUserId != request.UserId)
            return DataResult<UserDto>.Forbidden("Users may only delete their own api keys");

        var foundUser = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (foundUser is null)
            return DataResult<UserDto>.NotFound($"User with id '{request.UserId}' not found");

        var apiKeyToDelete = foundUser.ApiKeys
            .FirstOrDefault(ak => ak.Name == request.Name);

        if (apiKeyToDelete is null)
            return DataResult<UserDto>.NotFound($"User '{request.UserId}' has no api key named '{request.Name}'");

        var removeApiKeyResult = foundUser.RemoveApiKey(apiKeyToDelete.Key);
        if (removeApiKeyResult.IsFailure)
            return DataResult<UserDto>.Failure(removeApiKeyResult.Failures);

        await _unitOfWork.SaveChangesAsync();
        var dto = UserDto.FromDomain(foundUser);
        return DataResult<UserDto>.Success(dto);
    }
}