using AiPipeline.Orchestration.FileService.Domain.FileAggregate;

namespace AiPipeline.Orchestration.FileService.GrpcServer.Extensions;

public static class FileStorageScopeExtension
{
    public static StorageScope ToStorageScope(this FileStorageScope storageScope)
    {
        return storageScope switch
        {
            FileStorageScope.LongTerm => StorageScope.LongTerm,
            FileStorageScope.ShortTerm => StorageScope.ShortTerm,
            FileStorageScope.Temporary => StorageScope.Temporary,
            _ => throw new ArgumentOutOfRangeException(nameof(storageScope), storageScope, null)
        };
    }
}