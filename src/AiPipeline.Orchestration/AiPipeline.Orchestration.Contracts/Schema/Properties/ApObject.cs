using AiPipeline.Orchestration.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Contracts.Schema.Properties;

[ApElementType("ApObject")]
public record ApObject : IApElement
{
    public Dictionary<string, IApElement> Properties { get; }
    public List<string> NonRequiredProperties { get; }

    public ApObject(Dictionary<string, IApElement> properties, List<string>? nonRequiredProperties = null)
    {
        Properties = properties;
        NonRequiredProperties = nonRequiredProperties ?? [];
    }
}