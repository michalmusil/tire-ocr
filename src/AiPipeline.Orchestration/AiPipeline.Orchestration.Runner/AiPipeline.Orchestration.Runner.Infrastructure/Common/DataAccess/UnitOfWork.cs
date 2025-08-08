using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using AiPipeline.Orchestration.Runner.Application.User.Repositories;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    public INodeTypeEntityRepository NodeTypeRepository { get; }
    public IPipelineResultEntityRepository PipelineResultRepository { get; }
    public IUserEntityRepository UserRepository { get; }

    public UnitOfWork(INodeTypeEntityRepository nodeTypeRepository,
        IPipelineResultEntityRepository pipelineResultRepository,
        IUserEntityRepository userRepository)
    {
        NodeTypeRepository = nodeTypeRepository;
        PipelineResultRepository = pipelineResultRepository;
        UserRepository = userRepository;
    }

    public async Task SaveChangesAsync()
    {
        await NodeTypeRepository.SaveChangesAsync();
        await PipelineResultRepository.SaveChangesAsync();
        await UserRepository.SaveChangesAsync();
    }
}