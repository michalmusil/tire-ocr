using TireOcr.Shared.Persistence;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Repositories;

public interface IUserEntityRepository : IEntityRepository
{
    public Task<Domain.UserAggregate.User?> GetByIdAsync(Guid userId);
    public Task<Domain.UserAggregate.User?> GetByUsernameAsync(string username);
    public Task<Domain.UserAggregate.User?> GetByApiKeyStringAsync(string apiKeyString);
    public Task AddAsync(Domain.UserAggregate.User user);
    public Task RemoveAsync(Domain.UserAggregate.User user);
}