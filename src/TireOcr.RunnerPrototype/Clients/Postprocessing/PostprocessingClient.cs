using System.Net;
using TireOcr.RunnerPrototype.Dtos;
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
            res.EnsureSuccessStatusCode();
            var postprocessResult = await res.Content.ReadFromJsonAsync<TirePostprocessingResult>();
            return DataResult<TirePostprocessingResult>.Success(postprocessResult!);
        }
        catch (HttpRequestException ex)
        {
            return ex.StatusCode switch
            {
                HttpStatusCode.NotFound => DataResult<TirePostprocessingResult>.NotFound(
                    "No tire code was detected during Postprocessing."),
                _ => DataResult<TirePostprocessingResult>.Failure(DefaultFailure)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling Postprocess service.");
            return DataResult<TirePostprocessingResult>.Failure(DefaultFailure);
        }
    }
}