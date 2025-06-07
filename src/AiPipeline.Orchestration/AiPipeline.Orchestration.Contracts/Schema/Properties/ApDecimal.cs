using AiPipeline.Orchestration.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Contracts.Schema.Properties;

[ApElementType("ApDecimal")]
public record ApDecimal : IApElement
{
    public decimal Value { get; }

    public ApDecimal(decimal value)
    {
        Value = value;
    }
}