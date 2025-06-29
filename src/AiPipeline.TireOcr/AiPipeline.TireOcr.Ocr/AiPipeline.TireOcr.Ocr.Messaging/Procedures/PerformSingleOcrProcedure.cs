using AiPipeline.Orchestration.Shared.Contracts.Schema;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Properties;
using AiPipeline.Orchestration.Shared.Procedures;
using AiPipeline.TireOcr.Ocr.Messaging.Constants;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.Ocr.Messaging.Procedures;

public class PerformSingleOcrProcedure : IProcedure
{
    private const int _schemaVersion = 1;

    private static readonly IApElement _inputSchema =
        new ApFile(Guid.Empty, "", supportedContentTypes: ["image/jpeg", "image/png", "image/webp"]);

    private static readonly IApElement _outputSchema = new ApString("");

    public string Id => NodeMessagingConstants.PerformSingleOcrProcedureId;
    public int SchemaVersion => _schemaVersion;
    public IApElement InputSchema => _inputSchema;
    public IApElement OutputSchema => _outputSchema;

    public Task<DataResult<IApElement>> Execute(IApElement input)
    {
        return Task.FromResult(DataResult<IApElement>.Success(_outputSchema));
    }
}