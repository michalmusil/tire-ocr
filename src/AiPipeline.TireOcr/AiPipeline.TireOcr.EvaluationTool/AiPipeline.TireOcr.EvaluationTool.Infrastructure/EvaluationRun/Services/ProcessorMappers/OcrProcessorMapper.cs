using AiPipeline.TireOcr.EvaluationTool.Application.Common.Services;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Services.Processors.Ocr;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Services.ProcessorMappers;

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
            OcrType.QwenVl => _remoteServicesProcessor,
            OcrType.InternVl => _remoteServicesProcessor,
            _ => null
        };

        if (processor == null)
            return DataResult<IOcrProcessor>.Invalid($"Invalid OCR type {input}");

        return DataResult<IOcrProcessor>.Success(processor);
    }
}