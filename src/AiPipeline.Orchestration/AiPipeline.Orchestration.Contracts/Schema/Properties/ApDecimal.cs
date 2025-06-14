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

    public bool HasCompatibleSchemaWith(IApElement other)
    {
        return other is ApDecimal;
    }
    
    public List<T> GetAllDescendantsOfType<T>() where T : IApElement => [];
}