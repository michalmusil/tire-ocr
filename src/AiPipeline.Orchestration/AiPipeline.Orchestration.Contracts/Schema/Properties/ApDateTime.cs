using AiPipeline.Orchestration.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Contracts.Schema.Properties;

[ApElementType("ApDateTime")]
public record ApDateTime : IApElement
{
    public DateTime Value { get; }

    public ApDateTime(DateTime value)
    {
        Value = value;
    }

    public bool HasEquivalentSchemaWith(IApElement other)
    {
        return other is ApDateTime;
    }
}