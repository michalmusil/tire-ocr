using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Repositories;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using TireOcr.Shared.Pagination;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Repositories;

public class EvaluationRunEntityRepository : IEvaluationRunEntityRepository
{
    private readonly EvaluationToolDbContext _dbContext;

    public EvaluationRunEntityRepository(EvaluationToolDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync() => _dbContext.SaveChangesAsync();

    public async Task<PaginatedCollection<EvaluationRunEntity>> GetEvaluationRunsPaginatedAsync(
        PaginationParams pagination,
        string? searchTerm
    )
    {
        var st = searchTerm?.ToLower();

        var query = GetBasicQuery();
        if (st is not null)
            query = query.Where(r =>
                r.Title.ToLower().Contains(st)
            );
        query = query.OrderByDescending(er => er.UpdatedAt);

        return await query.ToPaginatedList(pagination);
    }

    public async Task<IEnumerable<EvaluationRunEntity>> GetEvaluationRunsByBatchIdAsync(Guid batchId)
    {
        return await GetBasicQuery()
            .Where(er => er.BatchId == batchId)
            .OrderByDescending(er => er.UpdatedAt)
            .ToListAsync();
    }

    public async Task<EvaluationRunEntity?> GetEvaluationRunByIdAsync(Guid id)
    {
        return await GetBasicQuery()
            .FirstOrDefaultAsync(er => er.Id == id);
    }

    public async Task Add(EvaluationRunEntity evaluationRun)
    {
        await _dbContext.EvaluationRuns.AddAsync(evaluationRun);
    }

    public Task AddRange(IEnumerable<EvaluationRunEntity> evaluationRuns)
    {
        return _dbContext.EvaluationRuns.AddRangeAsync(evaluationRuns);
    }

    public Task Remove(EvaluationRunEntity evaluationRun)
    {
        _dbContext.EvaluationRuns.Remove(evaluationRun);
        return Task.CompletedTask;
    }

    private IQueryable<EvaluationRunEntity> GetBasicQuery()
    {
        return _dbContext.EvaluationRuns
            .Include(er => er.Evaluation)
            .Include(er => er.PreprocessingResult)
            .Include(er => er.OcrResult)
            .Include(er => er.PostprocessingResult)
            .Include(er => er.DbMatchingResult);
    }
}