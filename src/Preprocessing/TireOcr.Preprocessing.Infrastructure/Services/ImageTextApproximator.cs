using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Infrastructure.Services;

public class ImageTextApproximator : IImageTextApproximator
{
    public DataResult<IEnumerable<string>> Approximate(IEnumerable<CharacterInImage> imageCharacters)
    {
        var characterList = imageCharacters.ToList();
        if (!characterList.Any())
            return DataResult<IEnumerable<string>>.NotFound("No characters found");

        var sortedLtr = characterList
            .OrderBy(charInImage => charInImage.TopLeftCoordinate.X)
            .ToList();
        var usedIndices = new HashSet<int>();
        var strings = new List<string>();

        for (int i = 0; i < sortedLtr.Count; i++)
        {
            if (!usedIndices.Add(i))
                continue;

            var currentString = sortedLtr[i].Character.ToString();
            var currentChar = sortedLtr[i];

            var nextCharFound = true;
            while (nextCharFound)
            {
                nextCharFound = false;
                for (int j = i + 1; j < sortedLtr.Count; j++)
                {
                    var investigatedIndex = j;
                    if (usedIndices.Contains(investigatedIndex))
                        continue;

                    if (currentChar.IsWithinVerticalSpanOf(sortedLtr[j]) && currentChar.IsLeftNeighborOf(sortedLtr[j]))
                    {
                        var charToAppend = sortedLtr[j].Character;
                        currentString += charToAppend;
                        currentChar = sortedLtr[j];
                        usedIndices.Add(investigatedIndex);
                        nextCharFound = true;
                        break;
                    }
                }
            }

            strings.Add(currentString);
        }
        
        return DataResult<IEnumerable<string>>.Success(strings);
    }
}