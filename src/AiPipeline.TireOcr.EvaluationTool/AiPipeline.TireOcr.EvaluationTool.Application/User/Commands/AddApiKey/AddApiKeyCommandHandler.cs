using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Services;
using AiPipeline.TireOcr.EvaluationTool.Domain.UserAggregate;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.AddApiKey;

public class AddApiKeyCommandHandler : ICommandHandler<AddApiKeyCommand, NewApiKeyDto>
{
    private const int ApiKeyByteLength = 256;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICryptographyService _cryptographyService;

    public AddApiKeyCommandHandler(IUnitOfWork unitOfWork, ICryptographyService cryptographyService)
    {
        _unitOfWork = unitOfWork;
        _cryptographyService = cryptographyService;
    }

    public async Task<DataResult<NewApiKeyDto>> Handle(AddApiKeyCommand request, CancellationToken cancellationToken)
    {
        if (request.CreatorUserId != request.UserId)
            return DataResult<NewApiKeyDto>.Forbidden("Users may only create api keys for themselves");

        var foundUser = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (foundUser is null)
            return DataResult<NewApiKeyDto>.NotFound($"User with id '{request.UserId}' not found");

        var keyString = _cryptographyService.GenerateCryptographicallyRandomString(ApiKeyByteLength);
        var newApiKey = new ApiKey(
            userId: request.UserId,
            name: request.Name,
            key: keyString,
            validUntil: request.ValidUntil
        );

        var addApiKeyResult = foundUser.AddApiKey(newApiKey);
        if (addApiKeyResult.IsFailure)
            return DataResult<NewApiKeyDto>.Failure(addApiKeyResult.Failures);

        await _unitOfWork.SaveChangesAsync();
        var dto = NewApiKeyDto.FromDomain(newApiKey);
        return DataResult<NewApiKeyDto>.Success(dto);
    }
}