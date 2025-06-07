using AiPipeline.Orchestration.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Contracts.Schema.Properties;

[ApElementType("ApInt")]
public record ApInt : IApElement
{
    public int Value { get; }

    public ApInt(int value)
    {
        Value = value;
    }
}