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

        if (HasDuplicatePropertyKeys(out var duplicateKey))
        {
            throw new ArgumentException(
                $"{nameof(ApObject)} can't contain duplicate properties. Duplicate property found: {duplicateKey}"
            );
        }

        if (HasNonRequiredPropertiesNotIncludedInProperties(out var notIncludedPropertyKey))
        {
            throw new ArgumentException(
                $"{nameof(ApObject)} can't contain non-required parameters not present in properties. " +
                $"Non-required parameter not present in properties: {notIncludedPropertyKey}"
            );
        }
    }

    public bool HasCompatibleSchemaWith(IApElement other)
    {
        if (other is not ApObject)
            return false;
        var otherObject = (other as ApObject)!;

        return HasEquivalentRequiredPropertiesWith(otherObject);
    }

    public List<T> GetAllDescendantsOfType<T>() where T : IApElement
    {
        var childrenOfType = Properties.Values
            .Where(item => item.GetType() == typeof(T))
            .Select(item => (T)item)
            .ToList();

        var descendantsOfType = Properties.Values
            .SelectMany(item => item.GetAllDescendantsOfType<T>())
            .ToList();

        return childrenOfType
            .Concat(descendantsOfType)
            .ToList();
    }

    private bool HasEquivalentRequiredPropertiesWith(ApObject other)
    {
        var thisRequiredPropertyPairs = Properties
            .Where(p => !NonRequiredProperties.Contains(p.Key))
            .OrderBy(p => p.Key)
            .ToList();
        var otherRequiredPropertyPairs = other.Properties
            .Where(p => !other.NonRequiredProperties.Contains(p.Key))
            .OrderBy(p => p.Key)
            .ToList();

        if (thisRequiredPropertyPairs.Count != otherRequiredPropertyPairs.Count)
            return false;

        for (var i = 0; i < thisRequiredPropertyPairs.Count; i++)
        {
            var a = thisRequiredPropertyPairs[i];
            var b = otherRequiredPropertyPairs[i];

            var propertyIsEquivalent = a.Key == b.Key && a.Value.HasCompatibleSchemaWith(b.Value);
            if (!propertyIsEquivalent)
                return false;
        }

        return true;
    }

    private bool HasDuplicatePropertyKeys(out string? duplicatePropertyKey)
    {
        var firstDuplicatePropertyKey = Properties.GroupBy(s => s.Key)
            .Where(g => g.Count() > 1)
            .Select(g => g.First().Key)
            .FirstOrDefault();

        duplicatePropertyKey = firstDuplicatePropertyKey;
        return firstDuplicatePropertyKey != null;
    }

    private bool HasNonRequiredPropertiesNotIncludedInProperties(out string? notIncludedPropertyKey)
    {
        var notIncluded = NonRequiredProperties.FirstOrDefault(nrp =>
            !Properties.Keys.Contains(nrp, StringComparer.InvariantCultureIgnoreCase));
        notIncludedPropertyKey = notIncluded;
        return notIncluded != null;
    }
}