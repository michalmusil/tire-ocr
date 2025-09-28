using System.Net.Http.Headers;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Exceptions;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.Processors.Preprocessing;

public class PreprocessingRoiExtractionProcessor : IPreprocessingProcessor
{
    private readonly HttpClient _remoteProcessorClient;
    private readonly ILogger<PreprocessingRoiExtractionProcessor> _logger;

    public PreprocessingRoiExtractionProcessor(HttpClient remoteProcessorClient,
        ILogger<PreprocessingRoiExtractionProcessor> logger)
    {
        _remoteProcessorClient = remoteProcessorClient;
        _logger = logger;
    }

    public async Task<DataResult<PreprocessingProcessorResult>> Process(ImageDto image,
        PreprocessingType preprocessingType)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(image.ImageData)
            {
                Headers =
                {
                    ContentType = MediaTypeHeaderValue.Parse(image.ContentType),
                    ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "Image",
                        FileName = image.FileName
                    }
                }
            });

            var res = await _remoteProcessorClient.PostAsync("/api/v1/Preprocess", content);
            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestExceptionWithContent(res.StatusCode, content: errorContent);
            }

            var imageData = await res.Content.ReadAsByteArrayAsync();
            var finalResult = new PreprocessingProcessorResult(
                Image: new ImageDto(image.FileName, image.ContentType, imageData),
                DurationMs: 0 // TODO: Add in remote preprocessing service
            );
            return DataResult<PreprocessingProcessorResult>.Success(finalResult);
        }
        catch (HttpRequestExceptionWithContent ex)
        {
            var statusCode = ex.StatusCode;
            ex.TryGetContentJsonProperty("detail", out var content);
            var failureMessage = $"Remote preprocessing failed: {content ?? "Failed to detect a tire code"}";

            _logger.LogError(ex, failureMessage);
            return DataResult<PreprocessingProcessorResult>.Failure(new Failure((int)statusCode!, failureMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling Preprocess service.");
            return DataResult<PreprocessingProcessorResult>.Failure(new Failure(500,
                $"Failed to preprocess image '{image.FileName}' due to unexpected error."));
        }
    }
}