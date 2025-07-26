using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Nodes.Services.FileReferenceUploader;

public class MinioFileReferenceUploaderService : IFileReferenceUploaderService
{
    private static string _providerName = "minio";
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioFileReferenceUploaderService> _logger;

    public MinioFileReferenceUploaderService(IMinioClient minioClient,
        ILogger<MinioFileReferenceUploaderService> logger)
    {
        _minioClient = minioClient;
        _logger = logger;
    }

    public async Task<DataResult<FileReference>> UploadFileDataAsync(Guid fileId, Stream fileStream, string contentType,
        string fileName, bool storePermanently)
    {
        var bucketName = storePermanently ? "long-term-files" : "temp-files";
        var filePath = $"{bucketName}/{fileName}";
        try
        {
            await EnsureBucketExistsAsync(bucketName);

            var args = new PutObjectArgs()
                .WithStreamData(fileStream)
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithContentType(contentType)
                .WithObjectSize(fileStream.Length);

            await _minioClient.PutObjectAsync(args);
            _logger.LogInformation($"Successfully uploaded '{fileName}' into minio bucket '{bucketName}'");

            var fileReference = new FileReference(
                Id: fileId,
                StorageProvider: _providerName,
                Path: filePath,
                ContentType: contentType
            );
            return DataResult<FileReference>.Success(fileReference);
        }
        catch (MinioException e)
        {
            _logger.LogError(
                $"Error while uploading file '{fileName}' to minio bucket '{bucketName}.': {e.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"Unexpected error while uploading file '{fileName}' to minio bucket '{bucketName}': {ex.Message}");
        }

        return DataResult<FileReference>.Failure(GetUploadFailure(fileName));
    }

    private Failure GetUploadFailure(string fileName) => new Failure(500, $"Failed to upload file '{fileName}'.");

    private async Task EnsureBucketExistsAsync(string bucketName)
    {
        var exists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (!exists)
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
    }
}