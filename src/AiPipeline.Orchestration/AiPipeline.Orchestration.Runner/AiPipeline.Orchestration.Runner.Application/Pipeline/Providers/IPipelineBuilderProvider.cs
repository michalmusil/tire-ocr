using AiPipeline.Orchestration.Runner.Application.Pipeline.Builders;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Providers;

public interface IPipelineBuilderProvider
{
    public IPipelineBuilder GetPipelineBuilder(Guid pipelineOwnerId);
    public IPipelineBatchBuilder GetPipelineBatchBuilder(Guid pipelineOwnerId);
}