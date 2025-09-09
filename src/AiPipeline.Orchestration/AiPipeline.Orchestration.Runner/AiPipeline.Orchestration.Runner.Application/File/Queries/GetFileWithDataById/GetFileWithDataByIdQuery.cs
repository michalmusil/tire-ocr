using AiPipeline.Orchestration.Runner.Application.File.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Queries.GetFileWithDataById;

public record GetFileWithDataByIdQuery(Guid Id, Guid UserId) : IQuery<GetFileWithDataStreamDto>;