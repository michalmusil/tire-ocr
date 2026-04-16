using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IQuery<UserDto>;