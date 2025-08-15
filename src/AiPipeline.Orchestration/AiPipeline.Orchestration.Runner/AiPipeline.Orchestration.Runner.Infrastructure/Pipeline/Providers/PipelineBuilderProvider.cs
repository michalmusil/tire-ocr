using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Builders;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Providers;
using AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Builders;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Providers;

public class PipelineBuilderProvider : IPipelineBuilderProvider
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileRepository _fileRepository;

    public PipelineBuilderProvider(IUnitOfWork unitOfWork, IFileRepository fileRepository)
    {
        _unitOfWork = unitOfWork;
        _fileRepository = fileRepository;
    }

    public IPipelineBuilder GetPipelineBuilder(Guid pipelineOwnerId) =>
        new PipelineBuilder(pipelineOwnerId, _unitOfWork, _fileRepository);

    public IPipelineBatchBuilder GetPipelineBatchBuilder(Guid pipelineOwnerId) =>
        new PipelineBatchBuilder(pipelineOwnerId, _unitOfWork, _fileRepository);
}