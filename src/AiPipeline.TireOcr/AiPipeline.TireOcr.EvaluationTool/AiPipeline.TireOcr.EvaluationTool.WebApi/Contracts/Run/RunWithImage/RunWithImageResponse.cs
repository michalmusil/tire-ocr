using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.RunWithImage;

public record RunWithImageResponse(
    EvaluationRunDto Result
);