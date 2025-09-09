using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;

[ApElementType("ApBool")]
public record ApBool : IApElement
{
    public bool Value { get; }

    public ApBool(bool value)
    {
        Value = value;
    }

    public static ApBool Template() => new(false);

    public bool HasCompatibleSchemaWith(IApElement other)
    {
        return other is ApBool;
    }

    public List<T> GetAllDescendantsOfType<T>() where T : IApElement => [];
}