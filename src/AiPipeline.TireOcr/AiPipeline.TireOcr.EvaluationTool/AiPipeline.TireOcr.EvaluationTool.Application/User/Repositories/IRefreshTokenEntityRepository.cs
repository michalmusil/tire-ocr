using AiPipeline.TireOcr.EvaluationTool.Domain.UserAggregate;
using TireOcr.Shared.Persistence;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Repositories;

public interface IRefreshTokenEntityRepository : IEntityRepository
{
    public Task<RefreshToken?> GetByTokenStringAsync(string tokenString);
}