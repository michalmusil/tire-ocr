using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;

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

    public IApElement this[string key]
    {
        get
        {
            if (Properties.TryGetValue(key, out var item))
                return item;
            throw new KeyNotFoundException($"The key '{key}' was not found in {nameof(ApObject)}s properties.");
        }
        set => Properties[key] = value;
    }

    public bool TryGetValueCaseInsensitive(string key, out IApElement? value)
    {
        var foundValues = Properties
            .Where(p => string.Equals(p.Key, key, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        if (foundValues.Length < 1)
        {
            value = null;
            return false;
        }
        value = foundValues[0].Value;
        return true;
    }

    public bool HasCompatibleSchemaWith(IApElement other)
    {
        if (other is not ApObject)
            return false;
        var otherObject = (other as ApObject)!;

        return HasEquivalentRequiredPropertiesWith(otherObject) && HasEquivalentNonRequiredPropertiesWith(otherObject);
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
        var thisRequiredPropertyPairs = GetOrderedPropertyPairs(this, required: true);
        var otherRequiredPropertyPairs = GetOrderedPropertyPairs(other, required: true);

        if (thisRequiredPropertyPairs.Count != otherRequiredPropertyPairs.Count)
            return false;

        for (var i = 0; i < thisRequiredPropertyPairs.Count; i++)
        {
            var a = thisRequiredPropertyPairs[i];
            var b = otherRequiredPropertyPairs[i];

            var propertyIsEquivalent = string.Equals(a.Key, b.Key, StringComparison.OrdinalIgnoreCase) &&
                                       a.Value.HasCompatibleSchemaWith(b.Value);
            if (!propertyIsEquivalent)
                return false;
        }

        return true;
    }

    private bool HasEquivalentNonRequiredPropertiesWith(ApObject other)
    {
        var thisNonRequiredPropertyPairs = GetOrderedPropertyPairs(this, required: false);
        var otherNonRequiredPropertyPairs = GetOrderedPropertyPairs(other, required: false);

        foreach (var thisPair in thisNonRequiredPropertyPairs)
        {
            var hasMatchInOther = otherNonRequiredPropertyPairs.Any(opp =>
                string.Equals(opp.Key, thisPair.Key, StringComparison.OrdinalIgnoreCase)
            );
            if (!hasMatchInOther)
                continue;

            var otherPair = otherNonRequiredPropertyPairs
                .First(opp => string.Equals(opp.Key, thisPair.Key, StringComparison.OrdinalIgnoreCase));
            var pairsSchemaMatches = thisPair.Value.HasCompatibleSchemaWith(otherPair.Value);
            if (!pairsSchemaMatches)
                return false;
        }

        return true;
    }

    private List<KeyValuePair<string, IApElement>> GetOrderedPropertyPairs(ApObject obj, bool required)
    {
        return obj.Properties
            .Where(p => required
                ? !NonRequiredProperties
                    .Any(nrp => string.Equals(nrp, p.Key, StringComparison.OrdinalIgnoreCase))
                : NonRequiredProperties.Any(nrp =>
                    string.Equals(nrp, p.Key, StringComparison.OrdinalIgnoreCase))
            )
            .OrderBy(p => p.Key)
            .ToList();
    }

    private bool HasDuplicatePropertyKeys(out string? duplicatePropertyKey)
    {
        var firstDuplicatePropertyKey = Properties.GroupBy(s => s.Key.ToLowerInvariant())
            .Where(g => g.Count() > 1)
            .Select(g => g.First().Key)
            .FirstOrDefault();

        duplicatePropertyKey = firstDuplicatePropertyKey;
        return firstDuplicatePropertyKey != null;
    }

    private bool HasNonRequiredPropertiesNotIncludedInProperties(out string? notIncludedPropertyKey)
    {
        var notIncluded = NonRequiredProperties.FirstOrDefault(nrp =>
            !Properties.Keys.Contains(nrp, StringComparer.OrdinalIgnoreCase));
        notIncludedPropertyKey = notIncluded;
        return notIncluded != null;
    }
}