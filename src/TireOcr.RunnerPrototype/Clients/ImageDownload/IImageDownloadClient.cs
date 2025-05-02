using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Clients.ImageDownload;

public interface IImageDownloadClient
{
    public Task<DataResult<Image>> DownloadImage(string imageUrl);

    public Task<IEnumerable<DataResult<Image>>> DownloadImageBatch(
        IEnumerable<string> imageUrls,
        bool sequentially = false
    );
}