using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using AiPipeline.Orchestration.Runner.Application.User.Repositories;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Queries.GetUserById;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserEntityRepository _repository;

    public GetUserByIdQueryHandler(IUserEntityRepository repository)
    {
        _repository = repository;
    }

    public async Task<DataResult<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}