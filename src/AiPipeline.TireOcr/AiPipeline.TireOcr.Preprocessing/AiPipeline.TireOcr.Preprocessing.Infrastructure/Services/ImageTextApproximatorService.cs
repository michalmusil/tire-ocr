using System.Text;
using Fastenshtein;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Infrastructure.Services;

public class ImageTextApproximatorService : IImageTextApproximatorService
{
    private readonly List<string> _referenceCodes =
        ["LT215/55R16 91V", "LT245/75R16 120/116Q", "P215/55ZR18 95V", "205/55R16 91V", "215/65ZR17 93W", "215/65ZR15"];

    private List<string> NormalizedReferenceCodes => _referenceCodes.Select(NormalizeTireCode).ToList();

    public DataResult<IEnumerable<StringInImage>> ApproximateStringsFromCharacters(
        IEnumerable<CharacterInImage> imageCharacters)
    {
        var characterList = imageCharacters.ToList();
        if (!characterList.Any())
            return DataResult<IEnumerable<StringInImage>>.NotFound("No characters found");

        var sortedLtr = characterList
            .OrderBy(charInImage => charInImage.TopLeftCoordinate.X)
            .ToList();
        var usedIndices = new HashSet<int>();
        var strings = new List<StringInImage>();

        for (int i = 0; i < sortedLtr.Count; i++)
        {
            if (!usedIndices.Add(i))
                continue;

            var currentString = sortedLtr[i].Character.ToString();
            var currentChar = sortedLtr[i];
            List<CharacterInImage> currentStringCharacters = [currentChar];

            var nextCharFound = true;
            while (nextCharFound)
            {
                nextCharFound = false;
                for (int j = i + 1; j < sortedLtr.Count; j++)
                {
                    var investigatedIndex = j;
                    if (usedIndices.Contains(investigatedIndex))
                        continue;

                    var investigatedCharacter = sortedLtr[j];

                    if (currentChar.IsWithinVerticalSpanOf(investigatedCharacter) &&
                        currentChar.IsLeftNeighborOf(investigatedCharacter))
                    {
                        var charToAppend = investigatedCharacter.Character;
                        currentString += charToAppend;
                        currentStringCharacters.Add(investigatedCharacter);
                        currentChar = investigatedCharacter;
                        usedIndices.Add(investigatedIndex);
                        nextCharFound = true;
                        break;
                    }
                }
            }

            strings.Add(new()
            {
                RawString = currentString,
                Characters = currentStringCharacters
            });
        }

        return DataResult<IEnumerable<StringInImage>>.Success(strings);
    }

    public DataResult<Dictionary<StringInImage, int>> GetTireCodeLevenshteinDistanceOfStrings(IEnumerable<StringInImage> strings)
    {
        var stringsList = strings.ToList();
        if (!stringsList.Any())
            return DataResult<Dictionary<StringInImage, int>>.Invalid("No strings for Levenshtein distance provided");

        var normalizedReferenceCodes = NormalizedReferenceCodes;
        Dictionary<StringInImage, int> results = new Dictionary<StringInImage, int>();

        foreach (var detected in stringsList)
        {
            var detectedNormalized = NormalizeTireCode(detected.RawString);
            var bestScore = int.MaxValue;
            var bestMatch = detected;

            foreach (var reference in normalizedReferenceCodes)
            {
                var lev = new Levenshtein(reference);
                var distance = lev.DistanceFrom(detectedNormalized);

                if (!detectedNormalized.Contains('/'))
                    distance *= 2;

                if (distance < bestScore)
                {
                    bestScore = distance;
                    bestMatch = detected;
                }
            }

            results[bestMatch] = bestScore;
        }

        return DataResult<Dictionary<StringInImage, int>>.Success(results);
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