using System.Text;
using Fastenshtein;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Infrastructure.Services;

public class ImageTextApproximator : IImageTextApproximator
{
    private readonly List<string> _referenceCodes =
        ["LT215/55R16 91V", "LT245/75R16 120/116Q", "P215/55ZR18 95V", "205/55R16 91V", "215/65ZR17 93W", "215/65ZR15"];

    private List<string> _normalizedReferenceCodes => _referenceCodes.Select(NormalizeTireCode).ToList();

    public DataResult<IEnumerable<string>> ApproximateStringsFromCharacters(
        IEnumerable<CharacterInImage> imageCharacters)
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

    public DataResult<Dictionary<string, int>> GetTireCodeLevenshteinDistanceOfStrings(IEnumerable<string> strings)
    {
        var stringsList = strings.ToList();
        if (!stringsList.Any())
            return DataResult<Dictionary<string, int>>.Invalid("No strings provided");

        var normalizedReferenceCodes = _normalizedReferenceCodes;
        Dictionary<string, int> results = new Dictionary<string, int>();

        foreach (string detected in stringsList)
        {
            var detectedNormalized = NormalizeTireCode(detected);
            var bestScore = int.MaxValue;
            var bestMatch = "";

            foreach (var reference in normalizedReferenceCodes)
            {
                var lev = new Levenshtein(reference);
                var distance = lev.DistanceFrom(detectedNormalized);

                if (!detectedNormalized.Contains('/'))
                    distance += (int)Math.Floor(distance * 0.3);

                if (distance < bestScore)
                {
                    bestScore = distance;
                    bestMatch = detected;
                }
            }

            results[bestMatch] = bestScore;
        }

        return DataResult<Dictionary<string, int>>.Success(results);
    }

    private string NormalizeTireCode(string input)
    {
        var noWhiteSpace = input.Replace(" ", "");
        var patternBuilder = new StringBuilder();

        foreach (var c in noWhiteSpace)
        {
            if (char.IsDigit(c))
                patternBuilder.Append('N');
            else if (char.IsLetter(c))
                patternBuilder.Append('L');
            else if (c == '/')
                patternBuilder.Append('/');
            else
                patternBuilder.Append('?');
        }

        return patternBuilder.ToString();
    }
}