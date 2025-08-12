using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using AiPipeline.Orchestration.Runner.Application.User.Repositories;

namespace AiPipeline.Orchestration.Runner.Application.Common.DataAccess;

public interface IUnitOfWork
{
    public INodeTypeEntityRepository NodeTypeRepository { get; }
    public IPipelineResultEntityRepository PipelineResultRepository { get; }
    public IUserEntityRepository UserRepository { get; }
    public IRefreshTokenEntityRepository RefreshTokenRepository { get; }
    public IApiKeyRepository ApiKeyRepository { get; }

    public Task SaveChangesAsync();
}