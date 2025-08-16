using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Infrastructure.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.Infrastructure.PipelineResult.Repositories;

public class PipelineResultEntityRepository : IPipelineResultEntityRepository
{
    private readonly OrchestrationRunnerDbContext _dbContext;

    public PipelineResultEntityRepository(OrchestrationRunnerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    public async Task<PaginatedCollection<Domain.PipelineResultAggregate.PipelineResult>>
        GetPipelineResultsPaginatedAsync(
            PaginationParams pagination, Guid userId)
    {
        var query = GetBasicQuery()
            .Where(pr => pr.UserId == userId)
            .OrderByDescending(pr => pr.UpdatedAt);

        return await query.ToPaginatedList(pagination);
    }

    public async Task<Domain.PipelineResultAggregate.PipelineResult?> GetPipelineResultByIdAsync(Guid id)
    {
        return await GetBasicQuery()
            .FirstOrDefaultAsync(pr => pr.Id == id);
    }

    public async Task<Domain.PipelineResultAggregate.PipelineResult?> GetPipelineResultByPipelineIdAsync(
        Guid pipelineId)
    {
        return await GetBasicQuery()
            .FirstOrDefaultAsync(pr => pr.PipelineId == pipelineId);
    }

    public async Task Add(Domain.PipelineResultAggregate.PipelineResult pipelineResult)
    {
        await _dbContext.PipelineResults
            .AddAsync(pipelineResult);
    }

    public Task AddRange(IEnumerable<Domain.PipelineResultAggregate.PipelineResult> pipelineResults)
    {
        return _dbContext.AddRangeAsync(pipelineResults);
    }

    public Task Remove(Domain.PipelineResultAggregate.PipelineResult pipelineResult)
    {
        _dbContext.PipelineResults
            .Remove(pipelineResult);
        return Task.CompletedTask;
    }

    private IQueryable<Domain.PipelineResultAggregate.PipelineResult> GetBasicQuery()
    {
        return _dbContext.PipelineResults
            .Include(nt => nt._stepResults);
    }
}