using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Services;

public interface IImageDownloadService
{
    /// <summary>
    /// Attempts to download an image with specified url.
    /// </summary>
    /// <param name="imageUrl">URL of image to download</param>
    /// <returns>DataResult containing either the downloaded image, or failure if download fails</returns>
    public Task<DataResult<ImageDto>> DownloadImageAsync(string imageUrl);

    /// <summary>
    /// Attempts to download all images specified by URLs on best effort basis - the method will return only
    /// images that were downloaded successfully. In case some of the download fail, they are simply excluded
    /// from result.
    /// </summary>
    /// <param name="imageUrls">A collection of image URLs to download</param>
    /// <param name="sequentially">If true, images will be downloaded sequentially instead of in parallel. Defaults to false</param>
    /// <returns>A dictionary where key is the original image URL, value is the downloaded image data`</returns>
    public Task<Dictionary<string, ImageDto>> DownloadImageBatchBestEffortAsync(
        IEnumerable<string> imageUrls,
        bool sequentially = false
    );

    /// <summary>
    /// Attempts to download all images specified by URLs - failure of one image download causes the entire method to fail.
    /// </summary>
    /// <param name="imageUrls">A collection of image URLs to download</param>
    /// <param name="sequentially">If true, images will be downloaded sequentially instead of in parallel. Defaults to false</param>
    /// <returns>A DataResult containing failure if just one of the download fails, or a dictionary with key as the original image URL and value as the downloaded image if all image downloads succeeded`</returns>
    public Task<DataResult<Dictionary<string, ImageDto>>> DownloadImageBatch(
        IEnumerable<string> imageUrls,
        bool sequentially = false
    );
}