using AiPipeline.TireOcr.EvaluationTool.Application.Common.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Services.Processors;

public interface IOcrProcessor
{
    public Task<DataResult<OcrResultEntity>> Process(ImageDto image, OcrType ocrType, PreprocessingType preprocessingType);
}