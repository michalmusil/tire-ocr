using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.Infrastructure.File.Repositories;

public class FileRepositoryFake : IFileRepository
{
    private static readonly List<Domain.FileAggregate.File> Files = [];

    public Task<PaginatedCollection<Domain.FileAggregate.File>> GetFilesPaginatedAsync(
        PaginationParams pagination, FileStorageScope? storageScope = null)
    {
        var matchingFiles = storageScope is null
            ? Files
            : Files
                .Where(f => f.FileStorageScope == storageScope)
                .ToList();
        return Task.FromResult(
            new PaginatedCollection<Domain.FileAggregate.File>(
                matchingFiles,
                matchingFiles.Count,
                pagination.PageNumber,
                pagination.PageSize)
        );
    }

    public Task<Domain.FileAggregate.File?> GetFileByIdAsync(Guid fileId)
    {
        return Task.FromResult(Files.FirstOrDefault(f => f.Id == fileId));
    }

    public Task<IEnumerable<Domain.FileAggregate.File>> GetFilesByIdsAsync(params Guid[] fileIds)
    {
        return Task.FromResult(Files.Where(f => fileIds.Contains(f.Id)));
    }

    public Task Add(Domain.FileAggregate.File file)
    {
        Files.Add(file);
        return Task.CompletedTask;
    }

    public Task Remove(Domain.FileAggregate.File file)
    {
        Files.Remove(file);
        return Task.CompletedTask;
    }

    public Task RemoveAllFilesWithIds(params Guid[] fileIds)
    {
        Files.RemoveAll(f => fileIds.Contains(f.Id));
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}