using AiPipeline.TireOcr.EvaluationTool.Application.Common.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Commands.RunSingleEvaluation;

public record RunSingleEvaluationCommand(
    ImageDto? InputImage,
    string? InputImageUrl,
    string? ExpectedTireCodeLabel,
    RunConfigDto RunConfig,
    Guid? RunId = null,
    string? RunTitle = null
) : ICommand<EvaluationRunDto>;