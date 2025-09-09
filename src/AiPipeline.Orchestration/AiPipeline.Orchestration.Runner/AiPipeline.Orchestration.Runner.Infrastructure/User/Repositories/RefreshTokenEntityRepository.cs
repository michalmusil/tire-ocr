using AiPipeline.Orchestration.Runner.Application.User.Repositories;
using AiPipeline.Orchestration.Runner.Domain.UserAggregate;
using AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AiPipeline.Orchestration.Runner.Infrastructure.User.Repositories;

public class RefreshTokenEntityRepository : IRefreshTokenEntityRepository
{
    private readonly OrchestrationRunnerDbContext _dbContext;

    public RefreshTokenEntityRepository(OrchestrationRunnerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync() => _dbContext.SaveChangesAsync();

    public async Task<RefreshToken?> GetByTokenStringAsync(string tokenString)
    {
        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rf => rf.Token == tokenString);
    }
}