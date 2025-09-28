using System.Net;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Services;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services;

public class ImageDownloaderService : IImageDownloadService
{
    private readonly HttpClient _client;
    private readonly IContentTypeValidationService _contentTypeValidationService;

    public ImageDownloaderService(HttpClient httpClient, IContentTypeValidationService contentTypeValidationService)
    {
        _client = httpClient;
        _contentTypeValidationService = contentTypeValidationService;
    }

    public async Task<DataResult<ImageDto>> DownloadImageAsync(string imageUrl)
    {
        try
        {
            using var response = await _client.GetAsync(imageUrl);
            if (!response.IsSuccessStatusCode)
            {
                return response.StatusCode switch
                {
                    HttpStatusCode.NotFound => DataResult<ImageDto>.NotFound($"Image {imageUrl} not found"),
                    HttpStatusCode.Unauthorized => DataResult<ImageDto>.Unauthorized(
                        $"Not authorized to download image {imageUrl}"),
                    _ => DataResult<ImageDto>.Failure(new Failure((int)response.StatusCode,
                        $"Failed to download image with provided url: {imageUrl}"))
                };
            }

            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (string.IsNullOrEmpty(contentType))
                return DataResult<ImageDto>.Failure(new Failure(500,
                    $"Could not determine content type of the image {imageUrl}"));

            if (!_contentTypeValidationService.IsContentTypeValid(contentType))
                return DataResult<ImageDto>.Invalid(
                    $"Image with url '{imageUrl}' has an unsupported content type of '{contentType}'. Content type must be one of {string.Join(',', _contentTypeValidationService.GetSupportedContentTypes())}"
                );

            var imageData = await response.Content.ReadAsByteArrayAsync();
            if (imageData.Length == 0)
                return DataResult<ImageDto>.Failure(new Failure(500,
                    $"Downloaded image with url '{imageUrl}' has no content"));

            var fileName = GetOriginalFileName(response.Content) ?? imageUrl;

            return DataResult<ImageDto>.Success(new ImageDto(fileName, contentType, imageData));
        }
        catch
        {
            return DataResult<ImageDto>.Failure(new Failure(500,
                $"Image download '{imageUrl}' failed due to unexpected error"));
        }
    }

    public async Task<Dictionary<string, DataResult<ImageDto>>> DownloadImageBatchAsync(IEnumerable<string> imageUrls,
        bool sequentially = false)
    {
        if (!sequentially)
        {
            var tasks = imageUrls
                .Select(async url =>
                {
                    var downloadResult = await DownloadImageAsync(url);
                    return KeyValuePair.Create(url, downloadResult);
                });
            var pairs = await Task.WhenAll(tasks);
            return pairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        var imageResults = new Dictionary<string, DataResult<ImageDto>>();
        imageUrls.ToList().ForEach(async void (url) =>
        {
            var result = await DownloadImageAsync(url);
            imageResults.Add(url,result);
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