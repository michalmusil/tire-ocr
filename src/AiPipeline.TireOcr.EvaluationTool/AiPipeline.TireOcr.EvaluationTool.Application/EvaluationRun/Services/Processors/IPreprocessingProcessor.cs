using AiPipeline.TireOcr.EvaluationTool.Application.Common.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Services.Processors;

public interface IPreprocessingProcessor
{
    public Task<DataResult<PreprocessingProcessorResult>> Process(ImageDto image, PreprocessingType preprocessingType);
}