using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using AiPipeline.Orchestration.Runner.Application.User.Repositories;

namespace AiPipeline.Orchestration.Runner.Application.Common.DataAccess;

public interface IUnitOfWork
{
    INodeTypeEntityRepository NodeTypeRepository { get; }
    IPipelineResultEntityRepository PipelineResultRepository { get; }
    IUserEntityRepository UserRepository { get; }

    Task SaveChangesAsync();
}