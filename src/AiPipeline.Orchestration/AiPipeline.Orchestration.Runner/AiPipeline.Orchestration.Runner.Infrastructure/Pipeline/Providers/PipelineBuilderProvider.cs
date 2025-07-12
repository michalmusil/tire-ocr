using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Builders;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Providers;
using AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Builders;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Providers;

public class PipelineBuilderProvider : IPipelineBuilderProvider
{
    private readonly IUnitOfWork _unitOfWork;

    public PipelineBuilderProvider(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IPipelineBuilder GetPipelineBuilder() => new PipelineBuilder(_unitOfWork);
}