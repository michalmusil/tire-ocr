using AiPipeline.TireOcr.EvaluationTool.Application.User.Repositories;
using AiPipeline.TireOcr.EvaluationTool.Domain.UserAggregate;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.User.Repositories;

public class RefreshTokenEntityRepository : IRefreshTokenEntityRepository
{
    private readonly EvaluationToolDbContext _dbContext;

    public RefreshTokenEntityRepository(EvaluationToolDbContext dbContext)
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