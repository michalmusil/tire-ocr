using TireOcr.RunnerPrototype.Dtos;
using TireOcr.RunnerPrototype.Exceptions;
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

    public async Task<DataResult<TirePostprocessingResult>> PostprocessTireCode(string rawTireCode)
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

            var postprocessResult = await res.Content.ReadFromJsonAsync<TirePostprocessingResult>();
            return DataResult<TirePostprocessingResult>.Success(postprocessResult!);
        }
        catch (HttpRequestExceptionWithContent ex)
        {
            var statusCode = ex.StatusCode;
            ex.TryGetContentJsonProperty("detail", out var content);
            var failureMessage = $"Postprocessing: {content ?? "No tire code was detected during Postprocessing"}";

            _logger.LogError(ex, failureMessage);
            return DataResult<TirePostprocessingResult>.Failure(new Failure((int)statusCode!, failureMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling Postprocess service.");
            return DataResult<TirePostprocessingResult>.Failure(DefaultFailure);
        }
    }
}