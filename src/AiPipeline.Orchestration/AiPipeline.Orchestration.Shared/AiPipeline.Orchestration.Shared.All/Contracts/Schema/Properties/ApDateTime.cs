using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;

[ApElementType("ApDateTime")]
public record ApDateTime : IApElement
{
    public DateTime Value { get; }

    public ApDateTime(DateTime value)
    {
        Value = value;
    }

    public bool HasCompatibleSchemaWith(IApElement other)
    {
        return other is ApDateTime;
    }
    
    public List<T> GetAllDescendantsOfType<T>() where T : IApElement => [];
}