using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    public INodeTypeEntityRepository NodeTypeEntityRepository { get; }
    public IPipelineResultEntityRepository PipelineResultEntityRepository { get; }

    public UnitOfWork(INodeTypeEntityRepository nodeTypeEntityRepository,
        IPipelineResultEntityRepository pipelineResultEntityRepository)
    {
        NodeTypeEntityRepository = nodeTypeEntityRepository;
        PipelineResultEntityRepository = pipelineResultEntityRepository;
    }

    public async Task SaveChangesAsync()
    {
        await NodeTypeEntityRepository.SaveChangesAsync();
        await PipelineResultEntityRepository.SaveChangesAsync();
    }
}