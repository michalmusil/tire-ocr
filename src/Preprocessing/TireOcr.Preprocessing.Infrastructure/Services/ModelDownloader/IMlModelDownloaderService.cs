using TireOcr.Preprocessing.Infrastructure.Models;

namespace TireOcr.Preprocessing.Infrastructure.Services.ModelDownloader;

public interface IMlModelDownloaderService
{
    public Task<bool> DownloadAsync(MlModel model);
}