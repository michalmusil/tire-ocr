using AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Infrastructure.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.Infrastructure.PipelineResultBatch.Repositories;

public class PipelineResultBatchEntityRepository : IPipelineResultBatchEntityRepository
{
    private readonly OrchestrationRunnerDbContext _dbContext;

    public PipelineResultBatchEntityRepository(OrchestrationRunnerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    public async Task<PaginatedCollection<Domain.PipelineResultBatchAggregate.PipelineResultBatch>>
        GetResultBatchesPaginatedAsync(PaginationParams pagination, Guid userId)
    {
        var query = GetBasicQuery()
            .Where(prb => prb.UserId == userId)
            .OrderByDescending(pr => pr.UpdatedAt);

        return await query.ToPaginatedList(pagination);
    }

    public async Task<Domain.PipelineResultBatchAggregate.PipelineResultBatch?> GetResultBatchByIdAsync(Guid id)
    {
        return await GetBasicQuery()
            .FirstOrDefaultAsync(prb => prb.Id == id);
    }

    public async Task Add(Domain.PipelineResultBatchAggregate.PipelineResultBatch pipelineResultBatch)
    {
        await _dbContext.PipelineResultBatches
            .AddAsync(pipelineResultBatch);
    }

    public Task Remove(Domain.PipelineResultBatchAggregate.PipelineResultBatch pipelineResultBatch)
    {
        _dbContext.PipelineResultBatches
            .Remove(pipelineResultBatch);
        
        return Task.CompletedTask;
    }

    private IQueryable<Domain.PipelineResultBatchAggregate.PipelineResultBatch> GetBasicQuery()
    {
        return _dbContext.PipelineResultBatches;
    }
}