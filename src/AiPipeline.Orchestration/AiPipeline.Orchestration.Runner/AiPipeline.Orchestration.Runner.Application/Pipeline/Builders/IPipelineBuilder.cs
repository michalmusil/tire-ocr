using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Builders;

public interface IPipelineBuilder
{
    public IApElement? PipelineInput { get; }
    public IReadOnlyCollection<RunPipelineStepDto> Steps { get; }
    public void SetPipelineInput(IApElement input);
    public void AddStep(RunPipelineStepDto step);

    public void AddSteps(List<RunPipelineStepDto> steps);

    public bool RemoveStep(RunPipelineStepDto step);

    public Task<DataResult<Domain.PipelineAggregate.Pipeline>> BuildAsync();
}