using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Queries.GetEvaluationRunBatchById;

public record GetEvaluationRunBatchByIdQuery(Guid Id) : IQuery<EvaluationRunBatchFullDto>;