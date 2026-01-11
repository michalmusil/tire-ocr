using System.Net.Http.Headers;
using System.Net.Http.Json;
using AiPipeline.TireOcr.EvaluationTool.Application.Common.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Dtos.RemoteOcrProcessor;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Exceptions;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Services.Processors.Ocr;

public class OcrRemotePythonProcessor : IOcrProcessor
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OcrRemotePythonProcessor> _logger;

    public OcrRemotePythonProcessor(HttpClient httpClient, ILogger<OcrRemotePythonProcessor> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DataResult<OcrResultEntity>> Process(ImageDto image, OcrType ocrType,
        PreprocessingType preprocessingType)
    {
        var endpointName = ocrType switch
        {
            OcrType.PaddleOcr => "paddle",
            OcrType.EasyOcr => "easy",
            _ => throw new ArgumentOutOfRangeException(nameof(ocrType), ocrType, null)
        };
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
                        Name = "image",
                        FileName = image.FileName
                    }
                }
            });
            if (preprocessingType == PreprocessingType.ExtractAndComposeSlices)
                content.Add(new StringContent("2"), "number_of_vertical_stacked_slices");

            var res = await _httpClient.PostAsync($"/ocr/{endpointName}", content);
            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestExceptionWithContent(res.StatusCode, content: errorContent);
            }

            var ocrResult = await res.Content.ReadFromJsonAsync<OcrServiceResponseDto>();

            return DataResult<OcrResultEntity>.Success(ocrResult!.ToDomain());
        }
        catch (HttpRequestExceptionWithContent ex)
        {
            var statusCode = ex.StatusCode;
            ex.TryGetContentJsonProperty("detail", out var content);
            var failureMessage =
                $"Remote python OCR service '{endpointName}' failed: {content ?? "No tire code was detected during Ocr"}";

            _logger.LogError(ex, failureMessage);
            return DataResult<OcrResultEntity>.Failure(new Failure((int)statusCode!, failureMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while calling remote python OCR processor endpoint '{endpointName}'.");
            return DataResult<OcrResultEntity>.Failure(new Failure(500,
                $"Failed to perform python OCR with endpoint '{endpointName}' on image '{image.FileName}' due to unexpected error."));
        }
    }
}