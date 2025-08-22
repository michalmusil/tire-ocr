using System.Text.RegularExpressions;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.All.Contracts.Schema.Selectors;

public class ChildElementSelector : IApElementSelector<IApElement>
{
    private const char Separator = '.';
    public string Selector { get; }

    private ChildElementSelector(string selector)
    {
        Selector = selector;
    }

    public static DataResult<ChildElementSelector> FromString(string stringSelector)
    {
        var validationResult = ValidateSelectorString(stringSelector);
        if (validationResult.IsFailure)
            return DataResult<ChildElementSelector>.Failure(validationResult.Failures);

        return DataResult<ChildElementSelector>.Success(new ChildElementSelector(stringSelector));
    }

    private static Result ValidateSelectorString(string stringSelector)
    {
        var trimmed = stringSelector.Trim();
        if (trimmed.Length == 0)
            return Result.Invalid("Selector can't be empty");

        var lastChar = trimmed[^1];
        if (lastChar == Separator)
            return Result.Invalid("Selector can't end with a separator");

        string pattern = $@"^([a-zA-Z0-9_@-]+\{Separator}?)*$";
        var isMatch = Regex.IsMatch(stringSelector, pattern);
        if (!isMatch)
            return Result.Invalid(
                $"Invalid selector '{stringSelector}. Selectors are key/index identifiers separated by separator '{Separator}' and must match following pattern: '{pattern}'");

        return Result.Success();
    }

    public DataResult<IApElement> Select(IApElement element)
    {
        IApElement loopedElement = element;
        var sections = Selector.Split(Separator);
        for (var i = 0; i < sections.Length; i++)
        {
            var section = sections[i];
            var isIndex = int.TryParse(section, out var apListIndex);
            if (isIndex)
            {
                if (loopedElement is ApList apList)
                {
                    if (apListIndex >= apList.Items.Count)
                        return DataResult<IApElement>.Invalid(
                            GetSelectorErrorMessage(i,
                                $"Attempted to get value at index '{apListIndex}', but length of the {nameof(ApList)} is only '{apList.Items.Count}'")
                        );

                    loopedElement = apList.Items[apListIndex];
                }
                else
                {
                    return DataResult<IApElement>.Invalid(
                        GetSelectorErrorMessage(i,
                            $"Attempted to get value at index '{apListIndex}', but the element at this depth is not {nameof(ApList)}, but {loopedElement.GetType().Name}")
                    );
                }
            }
            else
            {
                var key = section;
                if (loopedElement is ApObject apObject)
                {
                    var valueKey = apObject.Properties.Keys.FirstOrDefault(k =>
                        k.Equals(key, StringComparison.InvariantCultureIgnoreCase));
                    if (valueKey is null)
                        return DataResult<IApElement>.Invalid(
                            GetSelectorErrorMessage(i,
                                $"Attempted to get object value with key '{key}', but there is no matching property in {nameof(ApObject)}")
                        );

                    loopedElement = apObject.Properties[valueKey];
                }
                else
                {
                    return DataResult<IApElement>.Invalid(
                        GetSelectorErrorMessage(i,
                            $"Attempted to get object value with key '{key}', but the element at this depth is not {nameof(ApObject)}, but {loopedElement.GetType().Name}")
                    );
                }
            }
        }

        return DataResult<IApElement>.Success(loopedElement);
    }

    private string GetSelectorErrorMessage(int depth, string innerMessage) =>
        $"{nameof(ChildElementSelector)} - Failed to parse selector at depth [{depth}]: {innerMessage}";
}