using AiPipeline.Orchestration.FileService.Domain.FileAggregate;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Persistence;

namespace AiPipeline.Orchestration.FileService.Application.File.Repositories;

public interface IFileEntityRepository : IEntityRepository
{
    public Task<PaginatedCollection<Domain.FileAggregate.File>> GetFilesPaginatedAsync(
        PaginationParams pagination, Guid userId, FileStorageScope? storageScope = null
    );

    public Task<Domain.FileAggregate.File?> GetFileByIdAsync(Guid fileId);
    public Task<IEnumerable<Domain.FileAggregate.File>> GetFilesByIdsAsync(Guid? userId = null, params Guid[] fileIds);
    public Task Add(Domain.FileAggregate.File file);
    public Task Remove(Domain.FileAggregate.File file);
    public Task RemoveAllFilesWithIds(params Guid[] fileIds);
}