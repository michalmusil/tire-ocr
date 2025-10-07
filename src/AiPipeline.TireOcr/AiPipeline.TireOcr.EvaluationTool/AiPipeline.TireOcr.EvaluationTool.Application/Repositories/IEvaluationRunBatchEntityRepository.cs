using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Persistence;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Repositories;

public interface IEvaluationRunBatchEntityRepository : IEntityRepository
{
    public Task<PaginatedCollection<EvaluationRunBatchEntity>> GetEvaluationRunBatchesPaginatedAsync(
        PaginationParams pagination);

    public Task<EvaluationRunBatchEntity?> GetEvaluationRunBatchByIdAsync(Guid id, bool includeFullData);

    public Task Add(EvaluationRunBatchEntity batch);
    public Task AddRange(IEnumerable<EvaluationRunBatchEntity> batch);
    public Task Remove(EvaluationRunBatchEntity batch);
}