using AiPipeline.TireOcr.EvaluationTool.Domain.UserAggregate;
using TireOcr.Shared.Persistence;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Repositories;

public interface IApiKeyRepository : IEntityRepository
{
    public Task<ApiKey?> GetByKeyStringAsync(string key);
}