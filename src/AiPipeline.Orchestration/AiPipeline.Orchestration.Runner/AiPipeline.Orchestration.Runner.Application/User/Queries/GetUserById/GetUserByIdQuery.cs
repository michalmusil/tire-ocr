using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IQuery<UserDto>;