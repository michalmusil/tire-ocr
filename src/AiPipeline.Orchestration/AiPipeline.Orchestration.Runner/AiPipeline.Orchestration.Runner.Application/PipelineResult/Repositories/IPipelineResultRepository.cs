using TireOcr.Shared.Pagination;
using TireOcr.Shared.Persistence;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;

public interface IPipelineResultRepository: IRepository
{
    public Task<PaginatedCollection<Domain.PipelineResultAggregate.PipelineResult>> GetPipelineResultsPaginatedAsync(
        PaginationParams pagination
    );

    public Task<Domain.PipelineResultAggregate.PipelineResult?> GetPipelineResultByIdAsync(string id);
    public Task<Domain.PipelineResultAggregate.PipelineResult?> GetPipelineResultByPipelineIdAsync(string pipelineId);

    public Task Add(Domain.PipelineResultAggregate.PipelineResult pipelineResult);
    public Task Remove(Domain.PipelineResultAggregate.PipelineResult pipelineResult);
}