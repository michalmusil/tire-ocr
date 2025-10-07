using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Persistence;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Repositories;

public interface IEvaluationRunEntityRepository : IEntityRepository
{
    public Task<PaginatedCollection<EvaluationRunEntity>> GetEvaluationRunsPaginatedAsync(PaginationParams pagination);
    public Task<IEnumerable<EvaluationRunEntity>> GetEvaluationRunsByBatchIdAsync(Guid batchId);
    public Task<EvaluationRunEntity?> GetEvaluationRunByIdAsync(Guid id);

    public Task Add(EvaluationRunEntity evaluationRun);
    public Task AddRange(IEnumerable<EvaluationRunEntity> evaluationRuns);
    public Task Remove(EvaluationRunEntity evaluationRun);
}