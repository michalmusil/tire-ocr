using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Nodes.Services.FileReferenceDownloader;

public class MinioFileReferenceDownloaderService : IFileReferenceDownloaderService
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioFileReferenceDownloaderService> _logger;

    public MinioFileReferenceDownloaderService(IMinioClient minioClient,
        ILogger<MinioFileReferenceDownloaderService> logger)
    {
        _minioClient = minioClient;
        _logger = logger;
    }

    public async Task<DataResult<Stream>> DownloadFileReferenceDataAsync(FileReference reference)
    {
        var filePathParts = reference.Path.Split('/');

        if (filePathParts.Length < 2)
            return DataResult<Stream>.Invalid(
                $"Invalid file path for download: '{reference.Path}'. The path must contain at lease two parts separated by '/'. Can't download file '{reference.Id}'"
            );

        var objectName = filePathParts[^1];
        var bucketName = filePathParts[^2];

        try
        {
            var memoryStream = new MemoryStream();
            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream => { stream.CopyTo(memoryStream); });

            await _minioClient.GetObjectAsync(args);
            memoryStream.Position = 0;
            return DataResult<Stream>.Success(memoryStream);
        }
        catch (BucketNotFoundException)
        {
            return DataResult<Stream>.NotFound(
                $"Bucket with name '{bucketName}' not found. Can't download file '{reference.Id}'");
        }
        catch (ObjectNotFoundException)
        {
            return DataResult<Stream>.NotFound(
                $"Object with name '{objectName}' not found in bucket '{bucketName}'. Can't download file '{reference.Id}'");
        }
        catch (MinioException e)
        {
            return DataResult<Stream>.Failure(
                new Failure(500,
                    $"Minio error while downloading file '{reference.Id}' with name '{objectName}' from minio bucket '{bucketName}': {e.Message}")
            );
        }
        catch (Exception ex)
        {
            return DataResult<Stream>.Failure(
                new Failure(500,
                    $"Unexpected error while downloading file '{reference.Id}' with name '{objectName}' from minio bucket '{bucketName}': {ex.Message}")
            );
        }
    }
}