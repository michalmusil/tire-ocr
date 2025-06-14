using AiPipeline.Orchestration.Contracts.Schema;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Builders;

public interface IPipelineBuilder
{
    public IApElement? PipelineInput { get; }
    public IReadOnlyCollection<RunPipelineStepDto> Steps { get; }
    public IReadOnlyCollection<PipelineInputFileDto> Files { get; }
    public void SetPipelineInput(IApElement input);
    public void AddStep(RunPipelineStepDto step);

    public void AddSteps(List<RunPipelineStepDto> steps);

    public bool RemoveStep(RunPipelineStepDto step);

    public void AddFile(PipelineInputFileDto file);

    public void AddFiles(IEnumerable<PipelineInputFileDto> files);

    public bool RemoveFile(PipelineInputFileDto file);

    public Task<DataResult<Domain.PipelineAggregate.Pipeline>> BuildAsync();
}