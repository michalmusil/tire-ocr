using AiPipeline.TireOcr.EvaluationTool.Application.User.Repositories;
using AiPipeline.TireOcr.EvaluationTool.Domain.UserAggregate;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.User.Repositories;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly EvaluationToolDbContext _dbContext;

    public ApiKeyRepository(EvaluationToolDbContext dbContext)
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