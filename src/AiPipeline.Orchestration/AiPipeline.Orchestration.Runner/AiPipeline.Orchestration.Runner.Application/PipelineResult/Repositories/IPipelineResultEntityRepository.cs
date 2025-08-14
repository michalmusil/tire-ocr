using TireOcr.Shared.Pagination;
using TireOcr.Shared.Persistence;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;

public interface IPipelineResultEntityRepository : IEntityRepository
{
    public Task<PaginatedCollection<Domain.PipelineResultAggregate.PipelineResult>> GetPipelineResultsPaginatedAsync(
        PaginationParams pagination, Guid userId
    );

    public Task<Domain.PipelineResultAggregate.PipelineResult?> GetPipelineResultByIdAsync(Guid id);
    public Task<Domain.PipelineResultAggregate.PipelineResult?> GetPipelineResultByPipelineIdAsync(Guid pipelineId);

    public Task Add(Domain.PipelineResultAggregate.PipelineResult pipelineResult);
    public Task Remove(Domain.PipelineResultAggregate.PipelineResult pipelineResult);
}