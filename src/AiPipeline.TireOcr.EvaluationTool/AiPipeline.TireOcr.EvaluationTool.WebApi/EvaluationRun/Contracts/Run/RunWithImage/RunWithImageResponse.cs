using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRun.Contracts.Run.RunWithImage;

public record RunWithImageResponse(
    EvaluationRunDto Result
);