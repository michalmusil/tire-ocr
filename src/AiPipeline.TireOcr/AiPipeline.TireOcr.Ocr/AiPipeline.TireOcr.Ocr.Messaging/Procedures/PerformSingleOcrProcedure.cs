using AiPipeline.Orchestration.Shared.Contracts.Schema;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Properties;
using AiPipeline.Orchestration.Shared.Procedures;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.Ocr.Messaging.Procedures;

public class PerformSingleOcrProcedure : IProcedure
{
    private const string _name = "PerformSingleOcr";
    private const int _schemaVersion = 1;
    private static readonly IApElement _inputSchema = new ApFile("", "", "");
    private static readonly IApElement _outputSchema = new ApString("");

    public string Name => _name;
    public int SchemaVersion => _schemaVersion;
    public IApElement InputSchema => _inputSchema;
    public IApElement OutputSchema => _outputSchema;

    public Task<DataResult<IApElement>> Execute(IApElement input)
    {
        throw new NotImplementedException();
    }
}