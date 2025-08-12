using AiPipeline.Orchestration.Runner.Domain.UserAggregate;
using TireOcr.Shared.Persistence;

namespace AiPipeline.Orchestration.Runner.Application.User.Repositories;

public interface IApiKeyRepository : IEntityRepository
{
    public Task<ApiKey?> GetByKeyStringAsync(string key);
}