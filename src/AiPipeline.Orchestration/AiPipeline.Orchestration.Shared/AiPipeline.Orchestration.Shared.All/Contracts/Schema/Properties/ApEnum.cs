using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;

[ApElementType("ApEnum")]
public record ApEnum : IApElement
{
    public string Value { get; }
    public string[]? SupportedCases { get; set; }

    public ApEnum(string value, string[]? supportedCases)
    {
        Value = value;
        SupportedCases = supportedCases;
    }

    public static ApEnum Template(string[] supportedCases) => new("", supportedCases);

    public bool HasCompatibleSchemaWith(IApElement other)
    {
        if (other is not ApEnum otherAsEnum)
            return false;

        if (otherAsEnum.Value != "")
        {
            var othersCaseMismatch = SupportedCases?.Length > 0 && !SupportedCases
                .Contains(otherAsEnum.Value, StringComparer.OrdinalIgnoreCase);
            if (othersCaseMismatch)
                return false;
        }

        if (Value != "")
        {
            var thisCaseMissmatch = otherAsEnum.SupportedCases?.Length > 0 &&
                                    !otherAsEnum.SupportedCases
                                        .Contains(Value, StringComparer.OrdinalIgnoreCase);
            if (thisCaseMissmatch)
                return false;
        }

        return true;
    }

    public List<T> GetAllDescendantsOfType<T>() where T : IApElement => [];
}