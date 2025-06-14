using AiPipeline.Orchestration.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Contracts.Schema.Properties;

[ApElementType("ApString")]
public record ApString : IApElement
{
    public string Value { get; }

    public ApString(string value)
    {
        Value = value;
    }

    public bool HasCompatibleSchemaWith(IApElement other)
    {
        return other is ApString;
    }
    
    public List<T> GetAllDescendantsOfType<T>() where T : IApElement => [];
}