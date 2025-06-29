using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.Application.File.Repositories;

public interface IFileRepository
{
    public Task<PaginatedCollection<Domain.FileAggregate.File>> GetAllFilesOfStorageScopePaginatedAsync(
        PaginationParams pagination
    );

    public Task<Domain.FileAggregate.File?> GetFileByIdAsync(Guid fileId);
    public Task<Domain.FileAggregate.File?> GetFilesByIdsAsync(params Guid[] fileIds);
    public Task Add(Domain.FileAggregate.File file);
    public Task Remove(Domain.FileAggregate.File file);
    public Task RemoveAllFilesWithIds(params Guid[] fileIds);
    public Task SaveChangesAsync();
}