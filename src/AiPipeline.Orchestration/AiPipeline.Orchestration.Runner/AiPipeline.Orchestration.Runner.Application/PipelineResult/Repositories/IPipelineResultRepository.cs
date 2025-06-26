using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;

public interface IPipelineResultRepository
{
    public Task<PaginatedCollection<Domain.PipelineResultAggregate.PipelineResult>> GetPipelineResultsPaginatedAsync(
        PaginationParams pagination
    );

    public Task<Domain.PipelineResultAggregate.PipelineResult?> GetPipelineResultByIdAsync(string id);

    public Task Add(Domain.NodeTypeAggregate.NodeType nodeType);
    public Task Put(Domain.NodeTypeAggregate.NodeType nodeType);
    public Task Remove(Domain.NodeTypeAggregate.NodeType nodeType);
}