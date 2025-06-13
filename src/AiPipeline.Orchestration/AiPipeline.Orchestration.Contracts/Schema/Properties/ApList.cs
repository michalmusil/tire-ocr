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

    public bool HasEquivalentSchemaWith(IApElement other)
    {
        if (other is not ApList)
            return false;
        var otherAsApList = (other as ApList)!;
        
        var thisChildType = Items.FirstOrDefault()?.GetType();
        var otherChildType = otherAsApList.Items.FirstOrDefault()?.GetType();

        if (thisChildType is null && otherChildType is null)
            return true;
        
        Type x = typeof(ApList);
        
        return thisChildType == otherChildType;
    }
}