using AiPipeline.Orchestration.Runner.Domain.FileAggregate;

namespace AiPipeline.Orchestration.Runner.Infrastructure.File.Extensions;

public static class FileStorageScopeExtension
{
    public static string GetBucketName(this FileStorageScope fileStorageScope)
    {
        return fileStorageScope switch
        {
            FileStorageScope.LongTerm => "long-term-files",
            FileStorageScope.ShortTerm => "short-term-files",
            FileStorageScope.Temporary => "temp-files",
            _ => throw new ArgumentOutOfRangeException(nameof(fileStorageScope), fileStorageScope, null)
        };
    }
}