using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Builders;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Providers;
using AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Builders;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Providers;

public class PipelineBuilderProvider : IPipelineBuilderProvider
{
    private readonly INodeTypeRepository _nodeTypeRepository;
    private readonly IFileRepository _fileRepository;

    public PipelineBuilderProvider(INodeTypeRepository nodeTypeRepository, IFileRepository fileRepository)
    {
        _nodeTypeRepository = nodeTypeRepository;
        _fileRepository = fileRepository;
    }

    public IPipelineBuilder GetPipelineBuilder() => new PipelineBuilder(_nodeTypeRepository, _fileRepository);
}