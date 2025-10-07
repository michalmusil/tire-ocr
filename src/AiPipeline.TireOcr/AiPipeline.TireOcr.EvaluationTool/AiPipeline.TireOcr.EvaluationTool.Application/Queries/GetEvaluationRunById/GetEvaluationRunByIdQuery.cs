using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Queries.GetEvaluationRunById;

public record GetEvaluationRunByIdQuery(Guid Id) : IQuery<EvaluationRunDto>;