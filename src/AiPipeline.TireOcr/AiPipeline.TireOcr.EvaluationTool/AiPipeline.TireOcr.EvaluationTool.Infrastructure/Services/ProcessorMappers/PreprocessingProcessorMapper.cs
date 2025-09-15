using AiPipeline.TireOcr.EvaluationTool.Application.Services;
using AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.Processors.Preprocessing;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.ProcessorMappers;

public class PreprocessingProcessorMapper : IEnumToObjectMapper<PreprocessingType, IPreprocessingProcessor>
{
    private readonly PreprocessingResizeProcessor _resizeProcessor;
    private readonly PreprocessingRoiExtractionProcessor _roiExtractionProcessor;

    public PreprocessingProcessorMapper(PreprocessingResizeProcessor resizeProcessor,
        PreprocessingRoiExtractionProcessor roiExtractionProcessor)
    {
        _resizeProcessor = resizeProcessor;
        _roiExtractionProcessor = roiExtractionProcessor;
    }

    public DataResult<IPreprocessingProcessor> Map(PreprocessingType input)
    {
        IPreprocessingProcessor? processor = input switch
        {
            PreprocessingType.Resize => _resizeProcessor,
            PreprocessingType.ExtractRoi => _roiExtractionProcessor,
            PreprocessingType.ExtractRoiAndRemoveBg => _roiExtractionProcessor,
            _ => null
        };
        if (processor == null)
            return DataResult<IPreprocessingProcessor>.Invalid($"Invalid preprocessing type {input}");

        return DataResult<IPreprocessingProcessor>.Success(processor);
    }
}