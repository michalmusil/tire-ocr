using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;

public interface IPostprocessingProcessor
{
    public Task<DataResult<PostprocessingResultEntity>> Process(OcrResultEntity ocrResult,
        PostprocessingType postprocessingType);
}