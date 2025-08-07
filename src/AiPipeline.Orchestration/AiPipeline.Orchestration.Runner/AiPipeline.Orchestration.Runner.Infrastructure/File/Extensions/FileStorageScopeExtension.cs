using AiPipeline.Orchestration.Runner.Domain.FileAggregate;

namespace AiPipeline.Orchestration.Runner.Infrastructure.File.Extensions;

public static class FileStorageScopeExtension
{
    public static FileService.Domain.FileAggregate.FileStorageScope? ToGrpcServerStorageScope(
        this FileStorageScope? fileStorageScope)
    {
        return fileStorageScope switch
        {
            FileStorageScope.LongTerm => FileService.Domain.FileAggregate.FileStorageScope.LongTerm,
            FileStorageScope.ShortTerm => FileService.Domain.FileAggregate.FileStorageScope.ShortTerm,
            FileStorageScope.Temporary => FileService.Domain.FileAggregate.FileStorageScope.Temporary,
            null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(fileStorageScope), fileStorageScope, null)
        };
    }
}