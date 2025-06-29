using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using AiPipeline.Orchestration.Runner.Infrastructure.File.Extensions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace AiPipeline.Orchestration.Runner.Infrastructure.File.Repositories;

public class FileStorageMinioRepository : IFileStorageProviderRepository
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<FileStorageMinioRepository> _logger;

    public FileStorageMinioRepository(IMinioClient minioClient, ILogger<FileStorageMinioRepository> logger)
    {
        _minioClient = minioClient;
        _logger = logger;
    }

    public async Task<bool> UploadFileAsync(Stream fileStream, string contentType, FileStorageScope scope,
        string fileName, string? prefix = null)
    {
        var bucketName = scope.GetBucketName();
        var objectName = GetFullObjectName(fileName, prefix);
        try
        {
            await EnsureBucketExistsAsync(bucketName);

            var args = new PutObjectArgs()
                .WithStreamData(fileStream)
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithContentType(contentType)
                .WithObjectSize(fileStream.Length);

            var result = await _minioClient.PutObjectAsync(args);
            _logger.LogInformation($"Successfully uploaded '{objectName}' into minio bucket '{bucketName}'");
            return true;
        }
        catch (MinioException e)
        {
            _logger.LogError(
                $"Error while uploading file '{objectName}' to minio bucket '{bucketName}.': {e.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"Unexpected error while uploading file '{objectName}' to minio bucket '{bucketName}': {ex.Message}");
            throw;
        }
    }

    public async Task<Stream?> DownloadFileAsync(FileStorageScope scope, string fileName, string? prefix = null)
    {
        var bucketName = scope.GetBucketName();
        var objectName = GetFullObjectName(fileName, prefix);
        try
        {
            var memoryStream = new MemoryStream();
            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream => { stream.CopyTo(memoryStream); });

            await _minioClient.GetObjectAsync(args);
            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (BucketNotFoundException)
        {
            return null;
        }
        catch (ObjectNotFoundException)
        {
            return null;
        }
        catch (MinioException e)
        {
            _logger.LogError($"Error downloading file '{objectName}' from minio bucket '{bucketName}': {e.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"Unexpected error while downloading file '{objectName}' from minio bucket '{bucketName}': {ex.Message}");
            throw;
        }
    }

    public async Task<bool> RemoveFileAsync(FileStorageScope scope, string fileName, string? prefix = null)
    {
        var bucketName = scope.GetBucketName();
        var objectName = GetFullObjectName(fileName, prefix);
        try
        {
            var args = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);

            await _minioClient.RemoveObjectAsync(args);
            return true;
        }
        catch (MinioException e)
        {
            _logger.LogError($"Error removing file '{objectName}' from minio bucket '{bucketName}': {e.Message}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"Unexpected error while removing file '{objectName}' from minio bucket '{bucketName}': {ex.Message}");
            return false;
        }
    }

    public string GetProviderName() => "minio";

    public string GetFullFilePathFor(FileStorageScope scope, string fileName, string? prefix = null)
        => $"{scope.GetBucketName()}/{GetFullObjectName(fileName, prefix)}";

    private string GetFullObjectName(string fileName, string? prefix) =>
        prefix == null ? fileName : $"{prefix}/{fileName}";

    private async Task EnsureBucketExistsAsync(string bucketName)
    {
        var exists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (!exists)
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
    }
}