using AiPipeline.Orchestration.Shared.Contracts.Schema;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Properties;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.Postprocessing.Messaging.Procedures;

public class PerformTireCodePostprocessingProcedure
{
    private const string _name = "PerformTireCodePostprocessing";
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

    public string Name => _name;
    public int SchemaVersion => _schemaVersion;
    public IApElement InputSchema => _inputSchema;
    public IApElement OutputSchema => _outputSchema;

    public Task<DataResult<IApElement>> Execute(IApElement input)
    {
        throw new NotImplementedException();
    }
}