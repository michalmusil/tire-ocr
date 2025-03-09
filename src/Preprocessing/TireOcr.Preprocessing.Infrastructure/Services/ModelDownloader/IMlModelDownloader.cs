using TireOcr.Preprocessing.Infrastructure.Models;

namespace TireOcr.Preprocessing.Infrastructure.Services.ModelDownloader;

public interface IMlModelDownloader
{
    public Task<bool> DownloadAsync(MlModel model);
}