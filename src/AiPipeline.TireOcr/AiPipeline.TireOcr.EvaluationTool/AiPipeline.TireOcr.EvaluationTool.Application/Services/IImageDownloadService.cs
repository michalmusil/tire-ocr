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
    /// Attempts to download all images specified by URLs.
    /// </summary>
    /// <param name="imageUrls">A collection of image URLs to download</param>
    /// <param name="sequentially">If true, images will be downloaded sequentially instead of in parallel. Defaults to false</param>
    /// <returns>A dictionary with provided image URLs as keys and DataResults of given image downloads as values</returns>
    public Task<Dictionary<string, DataResult<ImageDto>>> DownloadImageBatchAsync(
        IEnumerable<string> imageUrls,
        bool sequentially = false
    );
}