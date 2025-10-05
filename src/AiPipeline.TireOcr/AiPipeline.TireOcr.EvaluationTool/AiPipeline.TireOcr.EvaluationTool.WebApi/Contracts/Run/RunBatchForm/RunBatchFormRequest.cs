using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.RunBatchForm;

public record RunBatchFormRequest(
    [Required] IFormFile ImageUrlsWithExpectedTireCodeLabelsCsv,
    [Required] PreprocessingType PreprocessingType,
    [Required] OcrType OcrType,
    [Required] PostprocessingType PostprocessingType,
    [Required] DbMatchingType DbMatchingType,
    [Required]
    [Range(1, 10)]
    [DefaultValue(5)]
    int ProcessingBatchSize,
    Guid? RunId = null,
    string? RunTitle = null
);