using System.Net.Http.Headers;
using System.Net.Http.Json;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemotePreprocessingProcessor;
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

            var removeBackground = preprocessingType switch
            {
                PreprocessingType.ExtractRoiAndRemoveBg => true,
                _ => false
            };
            content.Add(new StringContent(removeBackground.ToString()), "RemoveBackground");

            var res = await _remoteProcessorClient.PostAsync("/api/v1/Preprocess/ExtractRoi", content);
            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestExceptionWithContent(res.StatusCode, content: errorContent);
            }

            var responseContent = await res.Content.ReadFromJsonAsync<TirePreprocessingResponseDto>();
            var imageData = Convert.FromBase64String(responseContent!.Base64ImageData);
            var finalResult = new PreprocessingProcessorResult(
                EvaluationRunId: Guid.Empty,
                Image: new ImageDto(responseContent.FileName, responseContent.ContentType, imageData),
                DurationMs: responseContent.DurationMs
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