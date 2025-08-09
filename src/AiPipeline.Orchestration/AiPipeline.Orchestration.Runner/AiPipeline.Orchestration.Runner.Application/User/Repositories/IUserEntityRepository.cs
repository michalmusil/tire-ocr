using TireOcr.Shared.Persistence;

namespace AiPipeline.Orchestration.Runner.Application.User.Repositories;

public interface IUserEntityRepository : IEntityRepository
{
    public Task<Domain.UserAggregate.User?> GetByIdAsync(Guid userId);
    public Task AddAsync(Domain.UserAggregate.User user);
    public Task RemoveAsync(Domain.UserAggregate.User user);
}