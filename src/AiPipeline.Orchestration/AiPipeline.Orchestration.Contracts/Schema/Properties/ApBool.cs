using AiPipeline.Orchestration.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Contracts.Schema.Properties;

[ApElementType("ApBool")]
public record ApBool : IApElement
{
    public bool Value { get; }

    public ApBool(bool value)
    {
        Value = value;
    }
}