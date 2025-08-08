using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Application.User.Services;

public interface IAuthService
{
    public Task<DataResult<AccessRefreshTokenPair>> GetTokensForUserAsync(Domain.UserAggregate.User user);
    public Task<DataResult<Domain.UserAggregate.User>> GetUserFromAccessTokenAsync(string accessToken);
}