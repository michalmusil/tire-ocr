using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    public INodeTypeRepository NodeTypeRepository { get; }
    public IPipelineResultRepository PipelineResultRepository { get; }
    public IFileRepository FileRepository { get; }

    public UnitOfWork(INodeTypeRepository nodeTypeRepository, IPipelineResultRepository pipelineResultRepository,
        IFileRepository fileRepository)
    {
        NodeTypeRepository = nodeTypeRepository;
        PipelineResultRepository = pipelineResultRepository;
        FileRepository = fileRepository;
    }

    public async Task SaveChangesAsync()
    {
        await NodeTypeRepository.SaveChangesAsync();
        await PipelineResultRepository.SaveChangesAsync();
        await FileRepository.SaveChangesAsync();
    }
}