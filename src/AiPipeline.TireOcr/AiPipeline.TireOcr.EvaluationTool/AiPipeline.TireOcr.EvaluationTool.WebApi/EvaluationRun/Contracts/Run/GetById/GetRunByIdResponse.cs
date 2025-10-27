using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRun.Contracts.Run.GetById;

public record GetRunByIdResponse(
    EvaluationRunDto Item
);