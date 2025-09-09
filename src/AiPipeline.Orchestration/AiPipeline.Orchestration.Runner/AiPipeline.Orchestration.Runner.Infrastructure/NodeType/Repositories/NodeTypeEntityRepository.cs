using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Infrastructure.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.Infrastructure.NodeType.Repositories;

public class NodeTypeEntityRepository : INodeTypeEntityRepository
{
    private readonly OrchestrationRunnerDbContext _dbContext;

    public NodeTypeEntityRepository(OrchestrationRunnerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    public async Task<PaginatedCollection<Domain.NodeTypeAggregate.NodeType>> GetNodeTypesPaginatedAsync(
        PaginationParams pagination)
    {
        var query = GetBasicQuery()
            .OrderByDescending(nt => nt.UpdatedAt);

        return await query.ToPaginatedList(pagination);
    }

    public async Task<IEnumerable<Domain.NodeTypeAggregate.NodeType>> GetNodeTypesByIdsAsync(params string[] ids)
    {
        return await GetBasicQuery()
            .Where(nt => ids.Contains(nt.Id))
            .ToListAsync();
    }

    public async Task<Domain.NodeTypeAggregate.NodeType?> GetNodeTypeByIdAsync(string id)
    {
        return await GetBasicQuery()
            .FirstOrDefaultAsync(nt => nt.Id == id);
    }

    public async Task Add(Domain.NodeTypeAggregate.NodeType nodeType)
    {
        await _dbContext.NodeTypes
            .AddAsync(nodeType);
    }

    public async Task Put(Domain.NodeTypeAggregate.NodeType nodeType)
    {
        var exists = await GetBasicQuery()
            .AnyAsync(nt => nt.Id == nodeType.Id);
        if (!exists)
            await _dbContext.NodeTypes.AddAsync(nodeType);
        else
            _dbContext.NodeTypes.Update(nodeType);
    }

    public Task Remove(Domain.NodeTypeAggregate.NodeType nodeType)
    {
        _dbContext.NodeTypes
            .Remove(nodeType);
        return Task.CompletedTask;
    }

    private IQueryable<Domain.NodeTypeAggregate.NodeType> GetBasicQuery()
    {
        return _dbContext.NodeTypes
            .Include(nt => nt._availableProcedures);
    }
}