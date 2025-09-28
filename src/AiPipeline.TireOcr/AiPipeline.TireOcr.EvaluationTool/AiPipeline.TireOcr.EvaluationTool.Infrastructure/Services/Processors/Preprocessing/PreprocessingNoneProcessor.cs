using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.Processors.Preprocessing;

public class PreprocessingNoneProcessor : IPreprocessingProcessor
{
    public Task<DataResult<PreprocessingProcessorResult>>
        Process(ImageDto image, PreprocessingType preprocessingType) =>
        Task.FromResult(
            DataResult<PreprocessingProcessorResult>.Success(
                new PreprocessingProcessorResult(Image: image, DurationMs: 0)));
}