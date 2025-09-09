using TireOcr.RunnerPrototype.Dtos.DbMatching;
using TireOcr.RunnerPrototype.Dtos.Postprocessing;
using TireOcr.Shared.Exceptions;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Clients.DbMatching;

public class DbMatchingClient : IDbMatchingClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DbMatchingClient> _logger;

    private Failure DefaultFailure => new Failure(500, "Failed to get DB matches for processed tire code.");

    public DbMatchingClient(HttpClient httpClient, ILogger<DbMatchingClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DataResult<DbMatcherServiceResponseDto>> GetDbMatchesForTireCode(TirePostprocessingResultDto postprocessedTireCode)
    {
        try
        {
            var content = new
            {
                TireCode = postprocessedTireCode
            };
            var res = await _httpClient.PostAsJsonAsync("/api/v1/TireDbMatches", content);
            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestExceptionWithContent(res.StatusCode, content: errorContent);
            }

            var dbMatchingResult = await res.Content.ReadFromJsonAsync<DbMatcherServiceResponseDto>();
            return DataResult<DbMatcherServiceResponseDto>.Success(dbMatchingResult!);
        }
        catch (HttpRequestExceptionWithContent ex)
        {
            var statusCode = ex.StatusCode;
            ex.TryGetContentJsonProperty("detail", out var content);
            var failureMessage = $"DbMatching: {content ?? "Failed to get DB matches during DbMatching"}";

            _logger.LogError(ex, failureMessage);
            return DataResult<DbMatcherServiceResponseDto>.Failure(new Failure((int)statusCode!, failureMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling DbMatcher service.");
            return DataResult<DbMatcherServiceResponseDto>.Failure(DefaultFailure);
        }
    }
}