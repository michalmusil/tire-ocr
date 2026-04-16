using System.Net.Http.Json;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Exceptions;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.TasyDbMatcher.Infrastructure.Repositories;

public class RemoteTireParamsDbRepository : ITireParamsDbRepository
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _responseCache;
    private readonly ILogger<RemoteTireParamsDbRepository> _logger;

    public RemoteTireParamsDbRepository(HttpClient httpClient, IMemoryCache responseCache,
        ILogger<RemoteTireParamsDbRepository> logger)
    {
        _httpClient = httpClient;
        _responseCache = responseCache;
        _logger = logger;
    }

    public async Task<DataResult<IEnumerable<ProcessedTireParamsDatabaseEntryDto>>> GetAllTireParamEntries()
    {
        try
        {
            var key = _httpClient.BaseAddress?.AbsoluteUri ?? "";
            if (_responseCache.TryGetValue(key, out IEnumerable<ProcessedTireParamsDatabaseEntryDto>? result))
                return DataResult<IEnumerable<ProcessedTireParamsDatabaseEntryDto>>.Success(result!);

            var res = await _httpClient.GetAsync("");
            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestExceptionWithContent(res.StatusCode, content: errorContent);
            }

            var rawTireParams = await res.Content.ReadFromJsonAsync<List<RawTireParamsDatabaseEntryDto>>();
            var processedTireParams = rawTireParams!
                .Select(raw => new ProcessedTireParamsDatabaseEntryDto(raw))
                .ToList();
            _responseCache.Set(key, processedTireParams, CacheDuration);

            return DataResult<IEnumerable<ProcessedTireParamsDatabaseEntryDto>>.Success(processedTireParams);
        }
        catch (HttpRequestExceptionWithContent ex)
        {
            var statusCode = ex.StatusCode;
            ex.TryGetContentJsonProperty("detail", out var content);
            var failureMessage = $"TireDbMatching: {content ?? "Failed to get DB matches for processed tire code."}";
            _logger.LogError(ex, failureMessage);

            return DataResult<IEnumerable<ProcessedTireParamsDatabaseEntryDto>>.Failure(new Failure((int)statusCode!,
                failureMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception while getting Tire parameters from remote DB.");
            return DataResult<IEnumerable<ProcessedTireParamsDatabaseEntryDto>>.Failure(
                new Failure(500, "Failed to retrieve existing Tire database entries")
            );
        }
    }
}