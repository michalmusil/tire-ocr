using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;

public interface IPreprocessingProcessor
{
    public Task<DataResult<PreprocessingProcessorResult>> Process(ImageDto image, PreprocessingType preprocessingType);
}