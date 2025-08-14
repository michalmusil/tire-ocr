using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.Infrastructure.PipelineResult.Repositories;

public class PipelineResultEntityRepositoryFake : IPipelineResultEntityRepository
{
    private static List<Domain.PipelineResultAggregate.PipelineResult> _pipelineResults = [];

    public Task<PaginatedCollection<Domain.PipelineResultAggregate.PipelineResult>> GetPipelineResultsPaginatedAsync(
        PaginationParams pagination, Guid userId)
    {
        var results = _pipelineResults
            .Where(pr => pr.UserId == userId)
            .ToList();
        return Task.FromResult(
            new PaginatedCollection<Domain.PipelineResultAggregate.PipelineResult>(
                results,
                results.Count,
                pagination.PageNumber,
                pagination.PageSize)
        );
    }

    public Task<Domain.PipelineResultAggregate.PipelineResult?> GetPipelineResultByIdAsync(Guid id)
    {
        return Task.FromResult(_pipelineResults.FirstOrDefault(pr => pr.Id == id));
    }

    public Task<Domain.PipelineResultAggregate.PipelineResult?> GetPipelineResultByPipelineIdAsync(Guid pipelineId)
    {
        return Task.FromResult(_pipelineResults.FirstOrDefault(pr => pr.PipelineId == pipelineId));
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