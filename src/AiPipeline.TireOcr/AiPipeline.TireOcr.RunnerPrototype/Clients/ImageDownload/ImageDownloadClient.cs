using System.Net;
using TireOcr.RunnerPrototype.Dtos.Preprocessing;
using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Clients.ImageDownload;

public class ImageDownloadClient : IImageDownloadClient
{
    private readonly HttpClient _client;

    public ImageDownloadClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<ImageDownloadResultDto> DownloadImage(string imageUrl)
    {
        try
        {
            using var response = await _client.GetAsync(imageUrl);
            if (!response.IsSuccessStatusCode)
            {
                var failedResult = response.StatusCode switch
                {
                    HttpStatusCode.NotFound => DataResult<Image>.NotFound($"Image {imageUrl} not found"),
                    HttpStatusCode.Unauthorized => DataResult<Image>.Unauthorized(
                        $"Not authorized to download image {imageUrl}"),
                    _ => DataResult<Image>.Failure(new Failure((int)response.StatusCode,
                        $"Failed to download image with provided url: {imageUrl}"))
                };
                return new ImageDownloadResultDto(imageUrl, failedResult);
            }

            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (string.IsNullOrEmpty(contentType))
                return new ImageDownloadResultDto(
                    imageUrl,
                    DataResult<Image>.Invalid($"Could not determine content type of the image {imageUrl}")
                );

            var imageData = await response.Content.ReadAsByteArrayAsync();
            if (imageData.Length == 0)
                return new ImageDownloadResultDto(
                    imageUrl,
                    DataResult<Image>.Invalid($"Failed to read image data: {imageUrl}")
                );

            var fileName = GetOriginalFileName(response.Content) ?? imageUrl;

            return new ImageDownloadResultDto(
                imageUrl,
                DataResult<Image>.Success(new Image(imageData, fileName, contentType))
            );
        }
        catch
        {
            return new ImageDownloadResultDto(
                imageUrl,
                DataResult<Image>.Failure(new Failure(500, "Failed to download image"))
            );
        }
    }

    public async Task<IEnumerable<ImageDownloadResultDto>> DownloadImageBatch(
        IEnumerable<string> imageUrls,
        bool sequentially
    )
    {
        if (!sequentially)
        {
            var tasks = imageUrls.Select(DownloadImage);
            return await Task.WhenAll(tasks);
        }

        var imageResults = new List<ImageDownloadResultDto>();
        imageUrls.ToList().ForEach(async void (url) =>
        {
            var result = await DownloadImage(url);
            imageResults.Add(result);
        });

        return imageResults;
    }

    private string? GetOriginalFileName(HttpContent content)
    {
        var contentDisposition = content.Headers.ContentDisposition?.ToString();
        if (string.IsNullOrEmpty(contentDisposition))
            return null;

        var fileNameSegment = contentDisposition.Split(';')
            .FirstOrDefault(x =>
                x.Trim().StartsWith("filename*", StringComparison.OrdinalIgnoreCase) ||
                x.Trim().StartsWith("filename=", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(fileNameSegment))
            return null;

        if (!fileNameSegment.Contains("*=", StringComparison.OrdinalIgnoreCase))
            return fileNameSegment.Split('=', 2)[1].Trim('"');

        var encodedFilename = fileNameSegment.Split("*=", 2)[1];
        return Uri.UnescapeDataString(encodedFilename.Trim('"'));
    }
}