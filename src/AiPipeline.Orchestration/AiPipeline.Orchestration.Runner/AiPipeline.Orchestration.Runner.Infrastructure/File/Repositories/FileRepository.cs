using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Infrastructure.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.Infrastructure.File.Repositories;

public class FileRepository : IFileRepository
{
    private readonly OrchestrationRunnerDbContext _dbContext;

    public FileRepository(OrchestrationRunnerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    public async Task<PaginatedCollection<Domain.FileAggregate.File>> GetFilesPaginatedAsync(
        PaginationParams pagination,
        FileStorageScope? storageScope = null)
    {
        var query = GetBasicQuery()
            .Where(f => storageScope == null || f.FileStorageScope == storageScope)
            .OrderByDescending(f => f.UpdatedAt);

        return await query.ToPaginatedList(pagination);
    }

    public async Task<Domain.FileAggregate.File?> GetFileByIdAsync(Guid fileId)
    {
        return await GetBasicQuery()
            .FirstOrDefaultAsync(f => f.Id == fileId);
    }

    public async Task<IEnumerable<Domain.FileAggregate.File>> GetFilesByIdsAsync(params Guid[] fileIds)
    {
        return await GetBasicQuery()
            .Where(f => fileIds.Contains(f.Id))
            .ToListAsync();
    }

    public async Task Add(Domain.FileAggregate.File file)
    {
        await _dbContext.Files
            .AddAsync(file);
    }

    public Task Remove(Domain.FileAggregate.File file)
    {
        _dbContext.Files
            .Remove(file);
        return Task.CompletedTask;
    }

    public Task RemoveAllFilesWithIds(params Guid[] fileIds)
    {
        _dbContext.Files
            .RemoveRange(GetBasicQuery().Where(f => fileIds.Contains(f.Id)));
        return Task.CompletedTask;
    }

    private IQueryable<Domain.FileAggregate.File> GetBasicQuery()
    {
        return _dbContext.Files;
    }
}