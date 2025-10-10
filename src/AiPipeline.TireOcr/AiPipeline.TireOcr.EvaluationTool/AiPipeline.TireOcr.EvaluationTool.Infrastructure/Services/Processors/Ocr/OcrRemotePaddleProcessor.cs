using System.Net.Http.Headers;
using System.Net.Http.Json;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemoteOcrProcessor;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Exceptions;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.Processors.Ocr;

public class OcrRemotePaddleProcessor : IOcrProcessor
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OcrRemotePaddleProcessor> _logger;

    public OcrRemotePaddleProcessor(HttpClient httpClient, ILogger<OcrRemotePaddleProcessor> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DataResult<OcrResultEntity>> Process(ImageDto image, OcrType ocrType)
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
                        Name = "image",
                        FileName = image.FileName
                    }
                }
            });

            var res = await _httpClient.PostAsync("/ocr/paddle", content);
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
            var failureMessage = $"Remote PaddleOcr failed: {content ?? "No tire code was detected during Ocr"}";

            _logger.LogError(ex, failureMessage);
            return DataResult<OcrResultEntity>.Failure(new Failure((int)statusCode!, failureMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling remote PaddleOcr processor.");
            return DataResult<OcrResultEntity>.Failure(new Failure(500,
                $"Failed to perform PaddleOcr on image '{image.FileName}' due to unexpected error."));
        }
    }
}