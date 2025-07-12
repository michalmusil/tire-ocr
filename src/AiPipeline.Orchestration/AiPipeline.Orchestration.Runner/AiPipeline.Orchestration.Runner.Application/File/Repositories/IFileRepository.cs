using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Persistence;

namespace AiPipeline.Orchestration.Runner.Application.File.Repositories;

public interface IFileRepository: IRepository
{
    public Task<PaginatedCollection<Domain.FileAggregate.File>> GetFilesPaginatedAsync(
        PaginationParams pagination, FileStorageScope? storageScope = null
    );

    public Task<Domain.FileAggregate.File?> GetFileByIdAsync(Guid fileId);
    public Task<IEnumerable<Domain.FileAggregate.File>> GetFilesByIdsAsync(params Guid[] fileIds);
    public Task Add(Domain.FileAggregate.File file);
    public Task Remove(Domain.FileAggregate.File file);
    public Task RemoveAllFilesWithIds(params Guid[] fileIds);
}