using TireOcr.Preprocessing.Infrastructure.Extensions;
using TireOcr.Preprocessing.Infrastructure.Models;

namespace TireOcr.Preprocessing.Infrastructure.Services.ModelDownloader;

public class MlModelDownloader : IMlModelDownloader
{
    private readonly HttpClient _client;

    public MlModelDownloader(HttpClient client)
    {
        _client = client;
    }

    public async Task<bool> DownloadAsync(MlModel model)
    {
        try
        {
            using var response = await _client.GetAsync(model.DownloadLink);
            if (!response.IsSuccessStatusCode)
                return false;

            await using var fileStream =
                new FileStream(model.GetAbsolutePath(), FileMode.Create, FileAccess.Write, FileShare.None);
            await response.Content.CopyToAsync(fileStream);
            return true;
        }
        catch(Exception ex)
        {
            return false;
        }
    }
}