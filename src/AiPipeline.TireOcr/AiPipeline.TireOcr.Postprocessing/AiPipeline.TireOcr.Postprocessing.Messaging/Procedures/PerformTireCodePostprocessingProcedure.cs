using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;
using AiPipeline.Orchestration.Shared.Nodes.Procedures;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.Postprocessing.Messaging.Procedures;

public class PerformTireCodePostprocessingProcedure : IProcedure
{
    public string Id => "PerformTireCodePostprocessing";
    public int SchemaVersion => 1;
    public IApElement InputSchema => ApString.Template();
    public IApElement OutputSchema => new ApObject(
        properties: new()
        {
            { "RawCode", ApString.Template() },
            { "PostprocessedTireCode", ApString.Template() },
            { "VehicleClass", ApString.Template() },
            { "Width", ApDecimal.Template() },
            { "AspectRatio", ApDecimal.Template() },
            { "Construction", ApString.Template() },
            { "Diameter", ApDecimal.Template() },
            { "LoadIndex", ApString.Template() },
            { "SpeedRating", ApString.Template() }
        },
        nonRequiredProperties:
        [
            "VehicleClass", "Width", "AspectRatio", "Construction", "Diameter", "LoadIndex", "SpeedRating"
        ]
    );

    public Task<DataResult<IApElement>> ExecuteAsync(IApElement input, List<FileReference> fileReferences)
    {
        return Task.FromResult(DataResult<IApElement>.Success(OutputSchema));
    }
}