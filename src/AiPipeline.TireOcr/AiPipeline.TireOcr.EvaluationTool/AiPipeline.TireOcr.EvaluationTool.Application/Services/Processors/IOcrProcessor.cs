using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;

public interface IOcrProcessor
{
    public Task<DataResult<OcrResultValueObject>> Process(ImageDto image, OcrType ocrType);
}