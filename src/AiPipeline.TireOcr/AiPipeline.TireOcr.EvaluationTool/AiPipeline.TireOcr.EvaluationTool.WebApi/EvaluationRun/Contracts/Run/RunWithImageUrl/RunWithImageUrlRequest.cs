using System.ComponentModel.DataAnnotations;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRun.Contracts.Run.RunWithImageUrl;

public record RunWithImageUrlRequest(
    [Required] string ImageUrl,
    [Required] PreprocessingType PreprocessingType,
    [Required] OcrType OcrType,
    [Required] PostprocessingType PostprocessingType,
    [Required] DbMatchingType DbMatchingType,
    string? ExpectedTireCodeLabel,
    Guid? RunId = null,
    string? RunTitle = null
);