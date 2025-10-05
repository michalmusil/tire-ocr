using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.RunWithImageUrl;

public record RunWithImageUrlResponse(
    EvaluationRunDto Result
);