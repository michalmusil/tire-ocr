using AiPipeline.TireOcr.EvaluationTool.Application.User.Repositories;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.User.Repositories;

public class UserEntityRepository : IUserEntityRepository
{
    private readonly EvaluationToolDbContext _dbContext;

    public UserEntityRepository(EvaluationToolDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync() => _dbContext.SaveChangesAsync();

    public async Task<Domain.UserAggregate.User?> GetByIdAsync(Guid userId)
    {
        return await GetBaseQuery()
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<Domain.UserAggregate.User?> GetByUsernameAsync(string username)
    {
        return await GetBaseQuery()
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<Domain.UserAggregate.User?> GetByApiKeyStringAsync(string apiKeyString)
    {
        return await GetBaseQuery()
            .FirstOrDefaultAsync(u => u._apiKeys.Any(a => a.Key == apiKeyString));
    }

    public async Task AddAsync(Domain.UserAggregate.User user)
    {
        await _dbContext.Users
            .AddAsync(user);
    }

    public Task RemoveAsync(Domain.UserAggregate.User user)
    {
        _dbContext.Users
            .Remove(user);
        return Task.CompletedTask;
    }

    private IQueryable<Domain.UserAggregate.User> GetBaseQuery()
    {
        return _dbContext.Users
            .Include(u => u._refreshTokens)
            .Include(u => u._apiKeys);
    }
}