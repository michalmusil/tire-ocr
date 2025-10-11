using AiPipeline.TireOcr.EvaluationTool.Application.Services;
using AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.Processors.Ocr;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.ProcessorMappers;

public class OcrProcessorMapper : IEnumToObjectMapper<OcrType, IOcrProcessor>
{
    private readonly OcrRemoteServicesProcessor _remoteServicesProcessor;
    private readonly OcrRemotePaddleProcessor _remotePaddleProcessor;

    public OcrProcessorMapper(OcrRemoteServicesProcessor remoteServicesProcessor,
        OcrRemotePaddleProcessor remotePaddleProcessor)
    {
        _remoteServicesProcessor = remoteServicesProcessor;
        _remotePaddleProcessor = remotePaddleProcessor;
    }

    public DataResult<IOcrProcessor> Map(OcrType input)
    {
        IOcrProcessor? processor = input switch
        {
            OcrType.GoogleGemini => _remoteServicesProcessor,
            OcrType.MistralPixtral => _remoteServicesProcessor,
            OcrType.OpenAiGpt => _remoteServicesProcessor,
            OcrType.GoogleCloudVision => _remoteServicesProcessor,
            OcrType.AzureAiVision => _remoteServicesProcessor,
            OcrType.PaddleOcr => _remotePaddleProcessor,
            _ => null
        };

        if (processor == null)
            return DataResult<IOcrProcessor>.Invalid($"Invalid OCR type {input}");

        return DataResult<IOcrProcessor>.Success(processor);
    }
}