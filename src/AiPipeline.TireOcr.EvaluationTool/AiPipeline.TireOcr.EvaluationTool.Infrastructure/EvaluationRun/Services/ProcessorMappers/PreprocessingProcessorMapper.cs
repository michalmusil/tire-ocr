using AiPipeline.TireOcr.EvaluationTool.Application.Common.Services;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Services.Processors.Preprocessing;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Services.ProcessorMappers;

public class PreprocessingProcessorMapper : IEnumToObjectMapper<PreprocessingType, IPreprocessingProcessor>
{
    private readonly PreprocessingResizeProcessor _resizeProcessor;
    private readonly PreprocessingRoiExtractionProcessor _roiExtractionProcessor;
    private readonly PreprocessingSlicesCompositionProcessor _slicesCompositionProcessor;
    private readonly PreprocessingTextExtractionMosaicProcessor _textExtractionMosaicProcessor;
    private readonly PreprocessingSlicesEnhanceTextProcessor _slicesEnhanceTextProcessor;
    private readonly PreprocessingAbsoluteRoiProcessor _absoluteRoiProcessor;

    public PreprocessingProcessorMapper(PreprocessingResizeProcessor resizeProcessor,
        PreprocessingRoiExtractionProcessor roiExtractionProcessor,
        PreprocessingSlicesCompositionProcessor slicesCompositionProcessor,
        PreprocessingTextExtractionMosaicProcessor textExtractionMosaicProcessor,
        PreprocessingSlicesEnhanceTextProcessor slicesEnhanceTextProcessor,
        PreprocessingAbsoluteRoiProcessor absoluteRoiProcessor)
    {
        _resizeProcessor = resizeProcessor;
        _roiExtractionProcessor = roiExtractionProcessor;
        _slicesCompositionProcessor = slicesCompositionProcessor;
        _textExtractionMosaicProcessor = textExtractionMosaicProcessor;
        _slicesEnhanceTextProcessor = slicesEnhanceTextProcessor;
        _absoluteRoiProcessor = absoluteRoiProcessor;
    }

    public DataResult<IPreprocessingProcessor> Map(PreprocessingType input)
    {
        IPreprocessingProcessor? processor = input switch
        {
            PreprocessingType.Resize => _resizeProcessor,
            PreprocessingType.ExtractRoi => _roiExtractionProcessor,
            PreprocessingType.ExtractRoiAndEnhanceCharacters => _roiExtractionProcessor,
            PreprocessingType.ExtractAndComposeSlices => _slicesCompositionProcessor,
            PreprocessingType.ExtractTextsIntoMosaic => _textExtractionMosaicProcessor,
            PreprocessingType.ExtractAndComposeSlicesEnhanceText => _slicesEnhanceTextProcessor,
            PreprocessingType.ExtractAbsoluteRoi => _absoluteRoiProcessor,
            _ => null
        };
        if (processor == null)
            return DataResult<IPreprocessingProcessor>.Invalid($"Invalid preprocessing type {input}");

        return DataResult<IPreprocessingProcessor>.Success(processor);
    }
}