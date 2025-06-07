namespace AiPipeline.Orchestration.Contracts.Schema.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,  Inherited = false)]
public class ApElementTypeAttribute: Attribute
{
    public string TypeDiscriminator { get; }

    public ApElementTypeAttribute(string typeDiscriminator)
    {
        TypeDiscriminator = typeDiscriminator;
    }
}