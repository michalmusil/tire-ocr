using AiPipeline.Orchestration.FileService.Domain.FileAggregate;

namespace AiPipeline.Orchestration.FileService.GrpcServer.Extensions;

public static class StorageScopeExtension
{
    public static FileStorageScope? ToFileStorageScope(this StorageScope storageScope)
    {
        return storageScope switch
        {
            StorageScope.FileStorageScopeUnspecified => null,
            StorageScope.LongTerm => FileStorageScope.LongTerm,
            StorageScope.ShortTerm => FileStorageScope.ShortTerm,
            StorageScope.Temporary => FileStorageScope.Temporary,
            _ => throw new ArgumentOutOfRangeException(nameof(storageScope), storageScope, null)
        };
    }
}