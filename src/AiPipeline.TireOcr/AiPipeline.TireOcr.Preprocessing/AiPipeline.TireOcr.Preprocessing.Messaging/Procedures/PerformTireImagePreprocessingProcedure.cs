using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using AiPipeline.Orchestration.Shared.Nodes.Procedures;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.Preprocessing.Messaging.Procedures;

public class PerformTireImagePreprocessingProcedure : IProcedure
{
    public string Id => "PerformTireImagePreprocessing";
    public int SchemaVersion => 1;
    public IApElement InputSchema { get; }
    public IApElement OutputSchema { get; }

    public Task<DataResult<IApElement>> ExecuteAsync(IApElement input, List<FileReference> fileReferences)
    {
        throw new NotImplementedException();
    }
}