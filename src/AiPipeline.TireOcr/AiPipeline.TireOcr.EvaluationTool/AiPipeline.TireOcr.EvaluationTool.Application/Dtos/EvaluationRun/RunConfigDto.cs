using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;

public record RunConfigDto(
    PreprocessingType PreprocessingType,
    OcrType OcrType,
    PostprocessingType PostprocessingType,
    DbMatchingType DbMatchingType
);