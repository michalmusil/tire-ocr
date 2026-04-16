using System.Net.Http.Json;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Exceptions;

namespace AiPipeline.TireOcr.TasyDbMatcher.Infrastructure.Repositories;

public class SupportedManufacturersRemoteRepository : ISupportedManufacturersRepository
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _responseCache;
    private readonly ILogger<SupportedManufacturersRemoteRepository> _logger;

    public SupportedManufacturersRemoteRepository(HttpClient httpClient, IMemoryCache responseCache,
        ILogger<SupportedManufacturersRemoteRepository> logger)
    {
        _httpClient = httpClient;
        _responseCache = responseCache;
        _logger = logger;
    }

    public async Task<IEnumerable<SupportedManufacturerDto>> GetSupportedManufacturers()
    {
        try
        {
            var key = _httpClient.BaseAddress?.AbsoluteUri ?? "";
            if (_responseCache.TryGetValue(key, out IEnumerable<SupportedManufacturerDto>? result))
                return result!;

            var res = await _httpClient.GetAsync("");
            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestExceptionWithContent(res.StatusCode, content: errorContent);
            }

            var manufacturerNames = await res.Content.ReadFromJsonAsync<List<string>>();
            var manufacturers = manufacturerNames!
                .Select(name => new SupportedManufacturerDto(name))
                .ToList();
            _responseCache.Set(key, manufacturers, CacheDuration);

            return manufacturers;
        }
        catch (HttpRequestExceptionWithContent ex)
        {
            ex.TryGetContentJsonProperty("detail", out var content);
            var failureMessage = $"TireDbMatching: {content ?? "Failed to get DB matches for manufacturer."}";
            _logger.LogError(ex, failureMessage);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception while getting manufacturers from remote DB.");
            throw;
        }
    }
}