using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Queries.GetEvaluationRunById;

public record GetEvaluationRunByIdQuery(Guid Id) : IQuery<EvaluationRunDto>;