using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;

namespace AiPipeline.Orchestration.Runner.Application.Common.DataAccess;

public interface IUnitOfWork
{
    INodeTypeRepository NodeTypeRepository { get; }
    IPipelineResultRepository PipelineResultRepository { get; }
    IFileRepository FileRepository { get; }

    Task SaveChangesAsync();
}