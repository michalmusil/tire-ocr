using TireOcr.Shared.Pagination;
using TireOcr.Shared.Persistence;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Repositories;

public interface IPipelineResultBatchEntityRepository : IEntityRepository
{
    public Task<PaginatedCollection<Domain.PipelineResultBatchAggregate.PipelineResultBatch>>
        GetResultBatchesPaginatedAsync(PaginationParams pagination, Guid userId);

    public Task<Domain.PipelineResultBatchAggregate.PipelineResultBatch?> GetResultBatchByIdAsync(Guid id);

    public Task Add(Domain.PipelineResultBatchAggregate.PipelineResultBatch pipelineResultBatch);
    public Task Remove(Domain.PipelineResultBatchAggregate.PipelineResultBatch pipelineResultBatch);
}