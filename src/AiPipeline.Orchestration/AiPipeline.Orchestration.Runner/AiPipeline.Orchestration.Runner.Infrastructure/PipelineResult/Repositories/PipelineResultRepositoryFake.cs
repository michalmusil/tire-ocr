using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.Infrastructure.PipelineResult.Repositories;

public class PipelineResultRepositoryFake : IPipelineResultRepository
{
    private static List<Domain.PipelineResultAggregate.PipelineResult> _pipelineResults = [];

    public Task<PaginatedCollection<Domain.PipelineResultAggregate.PipelineResult>> GetPipelineResultsPaginatedAsync(
        PaginationParams pagination)
    {
        return Task.FromResult(
            new PaginatedCollection<Domain.PipelineResultAggregate.PipelineResult>(
                _pipelineResults,
                _pipelineResults.Count,
                pagination.PageNumber,
                pagination.PageSize)
        );
    }

    public Task<Domain.PipelineResultAggregate.PipelineResult?> GetPipelineResultByIdAsync(string id)
    {
        return Task.FromResult(_pipelineResults.FirstOrDefault(pr => pr.Id.ToString() == id));
    }

    public Task<Domain.PipelineResultAggregate.PipelineResult?> GetPipelineResultByPipelineIdAsync(string pipelineId)
    {
        return Task.FromResult(_pipelineResults.FirstOrDefault(pr => pr.PipelineId.ToString() == pipelineId));
    }

    public Task Add(Domain.PipelineResultAggregate.PipelineResult pipelineResult)
    {
        _pipelineResults.Add(pipelineResult);
        return Task.CompletedTask;
    }

    public Task Remove(Domain.PipelineResultAggregate.PipelineResult pipelineResult)
    {
        _pipelineResults.Remove(pipelineResult);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync()
    {
        return Task.FromResult(1);
    }
}