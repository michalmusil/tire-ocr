using AiPipeline.Orchestration.Runner.Application.User.Repositories;
using AiPipeline.Orchestration.Runner.Domain.UserAggregate;
using AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AiPipeline.Orchestration.Runner.Infrastructure.User.Repositories;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly OrchestrationRunnerDbContext _dbContext;

    public ApiKeyRepository(OrchestrationRunnerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync() => _dbContext.SaveChangesAsync();

    public async Task<ApiKey?> GetByKeyStringAsync(string tokenString)
    {
        return await _dbContext.ApiKeys
            .FirstOrDefaultAsync(rf => rf.Key == tokenString);
    }
}