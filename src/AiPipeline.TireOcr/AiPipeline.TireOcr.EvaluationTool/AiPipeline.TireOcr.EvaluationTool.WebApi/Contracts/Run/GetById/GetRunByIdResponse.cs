using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.GetById;

public record GetRunByIdResponse(
    EvaluationRunDto Item
);