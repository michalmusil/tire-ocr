using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Builders;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Providers;
using AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Builders;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Providers;

public class PipelineBuilderProvider : IPipelineBuilderProvider
{
    private readonly INodeTypeRepository _nodeTypeRepository;

    public PipelineBuilderProvider(INodeTypeRepository nodeTypeRepository)
    {
        _nodeTypeRepository = nodeTypeRepository;
    }

    public IPipelineBuilder GetPipelineBuilder() => new PipelineBuilder(_nodeTypeRepository);
}