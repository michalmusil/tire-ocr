using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;

public record TireCodeDto(
    string RawCode,
    string ProcessedCode,
    string? VehicleClass,
    decimal? Width,
    decimal? AspectRatio,
    string? Construction,
    decimal? Diameter,
    char? LoadRange,
    int? LoadIndex,
    int? LoadIndex2,
    string? SpeedRating
)
{
    public static TireCodeDto FromDomain(TireCodeValueObject domain) => new(
        RawCode: domain.RawCode,
        ProcessedCode: domain.ToString(),
        VehicleClass: domain.VehicleClass,
        Width: domain.Width,
        AspectRatio: domain.AspectRatio,
        Construction: domain.Construction,
        Diameter: domain.Diameter,
        LoadRange: domain.LoadRange,
        LoadIndex: domain.LoadIndex,
        LoadIndex2: domain.LoadIndex2,
        SpeedRating: domain.SpeedRating
    );

    public TireCodeValueObject ToDomain() => new()
    {
        RawCode = RawCode,
        VehicleClass = VehicleClass,
        Width = Width,
        AspectRatio = AspectRatio,
        Construction = Construction,
        Diameter = Diameter,
        LoadRange = LoadRange,
        LoadIndex = LoadIndex,
        LoadIndex2 = LoadIndex2,
        SpeedRating = SpeedRating
    };

    /// <summary>
    /// Extracts tire code from a predefined label string. Label string is a modified classic tire code (e.g.
    /// "LT 215/70R20 109/102T"), but spaces are replaced with '_' and slashes are replaced with "-", so the
    /// previously mentioned tire code in label string format would be: "LT_215-70R20_109-102T". 
    /// </summary>
    /// <param name="labelString">Label string in correct format described above</param>
    /// <returns>A DataResult containing parsed TireCodeDto if parsing is successful, failure otherwise.</returns>
    public static DataResult<TireCodeDto> FromLabelString(string labelString)
    {
        var failureResult = DataResult<TireCodeDto>.Invalid(
            $"Label string '{labelString}' is nod in valid format. Valid format is for example \"LT_215-70R20_109-102T\", which translates to \"LT 215/70R20 109/102T\"."
        );

        var parts = labelString.Split('_');
        if (parts.Length < 1)
            return failureResult;

        throw new NotImplementedException(); // TODO: Finish
    }
}