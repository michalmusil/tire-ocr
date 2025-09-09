using AiPipeline.Orchestration.Runner.Domain.UserAggregate;
using TireOcr.Shared.Persistence;

namespace AiPipeline.Orchestration.Runner.Application.User.Repositories;

public interface IRefreshTokenEntityRepository : IEntityRepository
{
    public Task<RefreshToken?> GetByTokenStringAsync(string tokenString);
}