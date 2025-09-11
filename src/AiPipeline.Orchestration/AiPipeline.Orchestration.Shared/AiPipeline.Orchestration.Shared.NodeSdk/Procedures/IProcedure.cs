using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.NodeSdk.Procedures;

public interface IProcedure
{
    public string Id { get; }
    public int SchemaVersion { get; }
    public IApElement InputSchema { get; }
    public IApElement OutputSchema { get; }

    public Task<DataResult<IApElement>> ExecuteAsync(RunPipelineStep step);
}