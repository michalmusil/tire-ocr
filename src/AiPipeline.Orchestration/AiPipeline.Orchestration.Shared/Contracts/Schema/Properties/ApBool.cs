using AiPipeline.Orchestration.Shared.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Shared.Contracts.Schema.Properties;

[ApElementType("ApBool")]
public record ApBool : IApElement
{
    public bool Value { get; }

    public ApBool(bool value)
    {
        Value = value;
    }

    public bool HasCompatibleSchemaWith(IApElement other)
    {
        return other is ApBool;
    }

    public List<T> GetAllDescendantsOfType<T>() where T : IApElement => [];
}