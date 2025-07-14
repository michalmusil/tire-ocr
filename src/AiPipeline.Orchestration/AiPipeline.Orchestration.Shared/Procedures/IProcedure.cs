using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Contracts.Schema;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Procedures;

public interface IProcedure
{
    public string Id { get; }
    public int SchemaVersion { get; }
    public IApElement InputSchema { get; }
    public IApElement OutputSchema { get; }

    public Task<DataResult<IApElement>> ExecuteAsync(IApElement input, List<FileReference> fileReferences);
}