using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;
using AiPipeline.Orchestration.Shared.Nodes.Procedures;
using AiPipeline.TireOcr.Postprocessing.Messaging.Constants;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.Postprocessing.Messaging.Procedures;

public class PerformTireCodePostprocessingProcedure : IProcedure
{
    private const int _schemaVersion = 1;
    private static readonly IApElement _inputSchema = new ApString("");

    private static readonly IApElement _outputSchema = new ApObject(
        properties: new()
        {
            { "RawCode", new ApString("") },
            { "PostprocessedTireCode", new ApString("") },
            { "VehicleClass", new ApString("") },
            { "Width", new ApDecimal(0m) },
            { "AspectRatio", new ApDecimal(0m) },
            { "Construction", new ApString("") },
            { "Diameter", new ApDecimal(0m) },
            { "LoadIndex", new ApString("") },
            { "SpeedRating", new ApString("") }
        },
        nonRequiredProperties:
        [
            "VehicleClass", "Width", "AspectRatio", "Construction", "Diameter", "LoadIndex", "SpeedRating"
        ]
    );

    public string Id => NodeMessagingConstants.PerformTireCodePostprocessingProcedureId;
    public int SchemaVersion => _schemaVersion;
    public IApElement InputSchema => _inputSchema;
    public IApElement OutputSchema => _outputSchema;

    public Task<DataResult<IApElement>> ExecuteAsync(IApElement input, List<FileReference> fileReferences)
    {
        return Task.FromResult(DataResult<IApElement>.Success(_outputSchema));
    }
}