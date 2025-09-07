using TireOcr.RunnerPrototype.Dtos.Preprocessing;

namespace TireOcr.RunnerPrototype.Clients.ImageDownload;

public interface IImageDownloadClient
{
    public Task<ImageDownloadResultDto> DownloadImage(string imageUrl);

    public Task<IEnumerable<ImageDownloadResultDto>> DownloadImageBatch(
        IEnumerable<string> imageUrls,
        bool sequentially = false
    );
}