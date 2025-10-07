using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Queries.GetEvaluationRunBatchById;

public record GetEvaluationRunBatchByIdQuery(Guid Id) : IQuery<EvaluationRunBatchFullDto>;