using TireOcr.RunnerPrototype.Dtos;

namespace TireOcr.RunnerPrototype.Clients.ImageDownload;

public interface IImageDownloadClient
{
    public Task<ImageDownloadResult> DownloadImage(string imageUrl);

    public Task<IEnumerable<ImageDownloadResult>> DownloadImageBatch(
        IEnumerable<string> imageUrls,
        bool sequentially = false
    );
}