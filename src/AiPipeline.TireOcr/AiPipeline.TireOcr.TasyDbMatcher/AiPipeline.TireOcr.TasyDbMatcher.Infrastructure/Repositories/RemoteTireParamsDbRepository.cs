using System.Net.Http.Json;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Repositories;
using TireOcr.Shared.Exceptions;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.TasyDbMatcher.Infrastructure.Repositories;

public class RemoteTireParamsDbRepository : ITireParamsDbRepository
{
    private readonly HttpClient _httpClient;

    public RemoteTireParamsDbRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DataResult<IEnumerable<ProcessedTireParamsDatabaseEntryDto>>> GetAllTireParamEntries()
    {
        try
        {
            var res = await _httpClient.GetAsync("/productStructure/variations");
            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestExceptionWithContent(res.StatusCode, content: errorContent);
            }

            var rawTireParams = (await res.Content.ReadFromJsonAsync<IEnumerable<RawTireParamsDatabaseEntryDto>>())!
                .ToList();
            var processedTireParams = rawTireParams.Select(raw => new ProcessedTireParamsDatabaseEntryDto(raw));

            return DataResult<IEnumerable<ProcessedTireParamsDatabaseEntryDto>>.Success(processedTireParams);
        }
        catch (HttpRequestExceptionWithContent ex)
        {
            var statusCode = ex.StatusCode;
            ex.TryGetContentJsonProperty("detail", out var content);
            var failureMessage = $"Ocr: {content ?? "No tire code was detected during Ocr"}";

            return DataResult<IEnumerable<ProcessedTireParamsDatabaseEntryDto>>.Failure(new Failure((int)statusCode!,
                failureMessage));
        }
        catch (Exception ex)
        {
            return DataResult<IEnumerable<ProcessedTireParamsDatabaseEntryDto>>.Failure(
                new Failure(500, "Failed to retrieve existing Tire database entries")
            );
        }
    }
}