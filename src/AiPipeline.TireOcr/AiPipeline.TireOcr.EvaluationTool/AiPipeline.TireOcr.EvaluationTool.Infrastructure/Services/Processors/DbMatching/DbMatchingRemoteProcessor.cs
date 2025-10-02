using System.Net.Http.Json;
using AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemoteDbMatchingProcessor;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Exceptions;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.Processors.DbMatching;

public class DbMatchingRemoteProcessor : IDbMatchingProcessor
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DbMatchingRemoteProcessor> _logger;

    public DbMatchingRemoteProcessor(HttpClient httpClient, ILogger<DbMatchingRemoteProcessor> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DataResult<DbMatchingResultValueObject>> Process(OcrResultValueObject ocrResult,
        TireCodeValueObject postprocessingResult, DbMatchingType dbMatchingType)
    {
        try
        {
            var content = new
            {
                TireCode = DbMatcherServiceRequestDto.FromDomain(postprocessingResult),
                Manufacturer = ocrResult.DetectedManufacturer
            };
            var res = await _httpClient.PostAsJsonAsync("/api/v1/TireDbMatches", content);
            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestExceptionWithContent(res.StatusCode, content: errorContent);
            }

            var dbMatchingResult = await res.Content.ReadFromJsonAsync<DbMatcherServiceResponseDto>();
            return DataResult<DbMatchingResultValueObject>.Success(dbMatchingResult!.ToDomain());
        }
        catch (HttpRequestExceptionWithContent ex)
        {
            var statusCode = ex.StatusCode;
            ex.TryGetContentJsonProperty("detail", out var content);
            var failureMessage =
                $"Remote dbMatching failure: {content ?? "Failed to get DB matches during DbMatching"}";

            _logger.LogError(ex, failureMessage);
            return DataResult<DbMatchingResultValueObject>.Failure(new Failure((int)statusCode!, failureMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling DbMatcher service.");
            return DataResult<DbMatchingResultValueObject>.Failure(new Failure(500,
                $"Failed to perform dbMatching on code '${postprocessingResult}' due to unexpected error."));
        }
    }
}