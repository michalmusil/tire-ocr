using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Repositories;
using AiPipeline.Orchestration.Runner.Application.User.Repositories;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly OrchestrationRunnerDbContext _dbContext;
    public INodeTypeEntityRepository NodeTypeRepository { get; }
    public IPipelineResultEntityRepository PipelineResultRepository { get; }
    public IUserEntityRepository UserRepository { get; }
    public IRefreshTokenEntityRepository RefreshTokenRepository { get; }
    public IApiKeyRepository ApiKeyRepository { get; }
    public IPipelineResultBatchEntityRepository PipelineResultBatchRepository { get; }

    public UnitOfWork(OrchestrationRunnerDbContext dbContext, INodeTypeEntityRepository nodeTypeRepository,
        IPipelineResultEntityRepository pipelineResultRepository, IUserEntityRepository userRepository,
        IRefreshTokenEntityRepository refreshTokenRepository, IApiKeyRepository apiKeyRepository,
        IPipelineResultBatchEntityRepository pipelineResultBatchRepository)
    {
        _dbContext = dbContext;
        NodeTypeRepository = nodeTypeRepository;
        PipelineResultRepository = pipelineResultRepository;
        UserRepository = userRepository;
        RefreshTokenRepository = refreshTokenRepository;
        ApiKeyRepository = apiKeyRepository;
        PipelineResultBatchRepository = pipelineResultBatchRepository;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}