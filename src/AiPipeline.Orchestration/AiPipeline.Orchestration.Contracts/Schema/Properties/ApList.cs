using AiPipeline.Orchestration.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Contracts.Schema.Properties;

[ApElementType("ApList")]
public record ApList : IApElement
{
    public List<IApElement> Items { get; }

    public ApList(List<IApElement> items)
    {
        var firstType = items.FirstOrDefault()?.GetType();
        if (firstType is not null)
        {
            var isValid = items.All(item => item.GetType() == firstType);
            if (!isValid)
                throw new ArgumentException($"Items of {nameof(ApList)} don't share the same type");
        }
        
        Items = items;
    }
}