using System.Net.Http.Json;
using AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemotePostprocessingProcessor;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Exceptions;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.Processors.Postprocessing;

public class PostprocessingRemoteServiceProcessor : IPostprocessingProcessor
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PostprocessingRemoteServiceProcessor> _logger;

    public PostprocessingRemoteServiceProcessor(HttpClient httpClient,
        ILogger<PostprocessingRemoteServiceProcessor> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DataResult<PostprocessingResultValueObject>> Process(OcrResultValueObject ocrResult,
        PostprocessingType postprocessingType)
    {
        try
        {
            var content = new
            {
                RawTireCode = ocrResult.DetectedCode
            };

            var res = await _httpClient.PostAsJsonAsync("/api/v1/Postprocess", content);
            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestExceptionWithContent(res.StatusCode, content: errorContent);
            }

            var postprocessResult = await res.Content.ReadFromJsonAsync<TirePostprocessingResponseDto>();
            return DataResult<PostprocessingResultValueObject>.Success(postprocessResult!.ToDomain());
        }
        catch (HttpRequestExceptionWithContent ex)
        {
            var statusCode = ex.StatusCode;
            ex.TryGetContentJsonProperty("detail", out var content);
            var failureMessage =
                $"Remote postprocessing failed: {content ?? "No tire code was detected during Postprocessing"}";

            _logger.LogError(ex, failureMessage);
            return DataResult<PostprocessingResultValueObject>.Failure(new Failure((int)statusCode!, failureMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling Postprocess service.");
            return DataResult<PostprocessingResultValueObject>.Failure(new Failure(500,
                $"Failed to postprocess code '{ocrResult.DetectedCode}' due to unexpected error."));
        }
    }
}