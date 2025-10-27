using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRun.Contracts.Run.RunWithImageUrl;

public record RunWithImageUrlResponse(
    EvaluationRunDto Result
);