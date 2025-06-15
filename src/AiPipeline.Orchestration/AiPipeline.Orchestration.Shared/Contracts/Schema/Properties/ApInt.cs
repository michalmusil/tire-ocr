using AiPipeline.Orchestration.Shared.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Shared.Contracts.Schema.Properties;

[ApElementType("ApInt")]
public record ApInt : IApElement
{
    public int Value { get; }

    public ApInt(int value)
    {
        Value = value;
    }

    public bool HasCompatibleSchemaWith(IApElement other)
    {
        return other is ApInt;
    }
    
    public List<T> GetAllDescendantsOfType<T>() where T : IApElement => [];
}