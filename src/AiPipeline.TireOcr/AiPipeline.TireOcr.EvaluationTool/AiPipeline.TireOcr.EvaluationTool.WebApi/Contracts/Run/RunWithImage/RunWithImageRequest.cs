using System.ComponentModel.DataAnnotations;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.RunWithImage;

public record RunWithImageRequest(
    [Required] IFormFile Image,
    [Required] PreprocessingType PreprocessingType,
    [Required] OcrType OcrType,
    [Required] PostprocessingType PostprocessingType,
    [Required] DbMatchingType DbMatchingType,
    string? ExpectedTireCode,
    Guid? RunId = null,
    string? RunTitle = null
);