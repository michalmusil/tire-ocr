using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Builders;

public interface IPipelineBatchBuilder
{
    public IReadOnlyCollection<IApElement> PipelineInputs { get; }
    public IReadOnlyCollection<RunPipelineStepDto> Steps { get; }
    public void AddInput(IApElement input);
    public void AddInputs(params IApElement[] inputs);
    public bool RemoveInput(IApElement input);

    public void AddStep(RunPipelineStepDto step);
    public void AddSteps(params RunPipelineStepDto[] steps);
    public bool RemoveStep(RunPipelineStepDto step);

    public Task<DataResult<PipelineBatch>> ValidateAndBuildAsync();
}