using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Services;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.RegisterUser;

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
        var newUserId = request.Id ?? Guid.NewGuid();
        var repository = _unitOfWork.UserRepository;

        var existingUserWithId = await repository.GetByIdAsync(newUserId);
        if (existingUserWithId is not null)
            return DataResult<UserDto>.Conflict("User with the same Id already exists");

        var existingUserWithUsername = await repository.GetByUsernameAsync(request.Username);
        if (existingUserWithUsername is not null)
            return DataResult<UserDto>.Conflict($"Username '{request.Username}' is already taken");

        var passwordHash = _hashService.GetHashOf(request.Password);
        if (passwordHash.IsFailure)
            return DataResult<UserDto>.Failure(passwordHash.Failures);

        var newUser = new Domain.UserAggregate.User(request.Username, passwordHash.Data!, id: newUserId);
        await repository.AddAsync(newUser);

        await _unitOfWork.SaveChangesAsync();
        var dto = UserDto.FromDomain(newUser);

        return DataResult<UserDto>.Success(dto);
    }
}