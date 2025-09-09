using AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Queries.GetResultBatchById;

public record GetResultBatchByIdQuery(Guid Id, Guid UserId) : IQuery<GetPipelineResultBatchDto>;