using TireOcr.RunnerPrototype.Dtos;
using TireOcr.Shared.Exceptions;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Clients.Postprocessing;

public class PostprocessingClient : IPostprocessingClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PostprocessingClient> _logger;

    private Failure DefaultFailure => new Failure(500, "Failed to postprocess image.");

    public PostprocessingClient(HttpClient httpClient, ILogger<PostprocessingClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DataResult<TirePostprocessingResultDto>> PostprocessTireCode(string rawTireCode)
    {
        try
        {
            var content = new
            {
                RawTireCode = rawTireCode
            };

            var res = await _httpClient.PostAsJsonAsync("/api/v1/Postprocess", content);
            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestExceptionWithContent(res.StatusCode, content: errorContent);
            }

            var postprocessResult = await res.Content.ReadFromJsonAsync<TirePostprocessingResultDto>();
            return DataResult<TirePostprocessingResultDto>.Success(postprocessResult!);
        }
        catch (HttpRequestExceptionWithContent ex)
        {
            var statusCode = ex.StatusCode;
            ex.TryGetContentJsonProperty("detail", out var content);
            var failureMessage = $"Postprocessing: {content ?? "No tire code was detected during Postprocessing"}";

            _logger.LogError(ex, failureMessage);
            return DataResult<TirePostprocessingResultDto>.Failure(new Failure((int)statusCode!, failureMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling Postprocess service.");
            return DataResult<TirePostprocessingResultDto>.Failure(DefaultFailure);
        }
    }
}