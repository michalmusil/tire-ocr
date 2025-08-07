using AiPipeline.Orchestration.FileService.GrpcSdk.Clients.FileServiceClient;
using AiPipeline.Orchestration.FileService.GrpcServer;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.Infrastructure.File.Repositories;

public class FileRepositoryGrpc : IFileRepository
{
    private readonly IFileSerivceClient _grpcClient;

    public FileRepositoryGrpc(IFileSerivceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public Task<int> SaveChangesAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<PaginatedCollection<Domain.FileAggregate.File>> GetFilesPaginatedAsync(
        PaginationParams pagination,
        FileStorageScope? storageScope = null)
    {
        var request = new GetAllFilesRequest
        {
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            StorageScopeFilter = GetStorageScope(storageScope)
        };
        var remoteResult = await _grpcClient.GetAllFilesPaginatedAsync(request);
        if (remoteResult.IsFailure)
            remoteResult.PrimaryFailure!.ThrowAsException();

        var remoteData = remoteResult.Data!;
        var rp = remoteData.Pagination;

        var localItems = remoteData.Items
            .Select(rf => new Domain.FileAggregate.File(
                    new Guid(rf.FileGuid),
                    GetFileStorageScope(rf.FileStorageScope) ?? FileStorageScope.Temporary,
                    rf.StorageProvider,
                    rf.Path,
                    rf.ContentType
                )
            ).ToList();
        var localData = new PaginatedCollection<Domain.FileAggregate.File>
        {
            Items = localItems,
            Pagination = new TireOcr.Shared.Pagination.Pagination(
                PageNumber: rp.PageNumber,
                PageSize: rp.PageSize,
                TotalCount: rp.TotalCount,
                TotalPages: rp.TotalPages,
                HasNextPage: rp.HasNextPage,
                HasPreviousPage: rp.HasPreviousPage
            )
        };

        return localData;
    }

    public Task<Domain.FileAggregate.File?> GetFileByIdAsync(Guid fileId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Domain.FileAggregate.File>> GetFilesByIdsAsync(params Guid[] fileIds)
    {
        throw new NotImplementedException();
    }

    public Task Add(Domain.FileAggregate.File file)
    {
        throw new NotImplementedException();
    }

    public Task Remove(Domain.FileAggregate.File file)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAllFilesWithIds(params Guid[] fileIds)
    {
        throw new NotImplementedException();
    }
}