using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Commands.RunSingleEvaluation;

public record RunSingleEvaluationCommand(
    ImageDto InputImage,
    TireCodeValueObject? ExpectedTireCode,
    PreprocessingType PreprocessingType,
    OcrType OcrType,
    PostprocessingType PostprocessingType,
    DbMatchingType DbMatchingType
) : ICommand<EvaluationRunDto>;