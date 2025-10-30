using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Repositories;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using TireOcr.Shared.Pagination;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRunBatch.Repositories;

public class EvaluationRunBatchEntityRepository : IEvaluationRunBatchEntityRepository
{
    private readonly EvaluationToolDbContext _dbContext;

    public EvaluationRunBatchEntityRepository(EvaluationToolDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync() => _dbContext.SaveChangesAsync();

    public async Task<PaginatedCollection<EvaluationRunBatchLightDto>> GetEvaluationRunBatchesPaginatedAsync(
        PaginationParams pagination,
        string? searchTerm
    )
    {
        var st = searchTerm?.ToLower();

        IQueryable<EvaluationRunBatchEntity> initialQuery = _dbContext.EvaluationRunBatches;
        if (st is not null)
            initialQuery = initialQuery.Where(b =>
                b.Title.ToLower().Contains(st)
            );


        var selectQuery = initialQuery
            .OrderByDescending(erb => erb.CreatedAt)
            .Select(erb => new EvaluationRunBatchLightDto(
                erb.Id.ToString(),
                erb.Title,
                erb._evaluationRuns.Count,
                erb.CreatedAt,
                erb._evaluationRuns.Count > 0 ? erb._evaluationRuns.Min(e => e.StartedAt) : null,
                erb._evaluationRuns.Count > 0 ? erb._evaluationRuns.Max(e => e.FinishedAt) : null
            ));

        return await selectQuery.ToPaginatedList(pagination);
    }

    public async Task<EvaluationRunBatchEntity?> GetEvaluationRunBatchByIdAsync(Guid id)
    {
        return await _dbContext.EvaluationRunBatches
            .FirstOrDefaultAsync(erb => erb.Id == id);
    }

    public async Task Add(EvaluationRunBatchEntity batch)
    {
        await _dbContext.EvaluationRunBatches.AddAsync(batch);
    }

    public Task AddRange(IEnumerable<EvaluationRunBatchEntity> batch)
    {
        return _dbContext.EvaluationRunBatches.AddRangeAsync(batch);
    }

    public Task Remove(EvaluationRunBatchEntity batch)
    {
        _dbContext.EvaluationRunBatches.Remove(batch);
        return Task.CompletedTask;
    }
}