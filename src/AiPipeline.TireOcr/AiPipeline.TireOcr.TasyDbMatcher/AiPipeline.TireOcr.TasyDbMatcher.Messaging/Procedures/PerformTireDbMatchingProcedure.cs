using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;
using AiPipeline.Orchestration.Shared.Nodes.Procedures;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Queries.GetTasyDbEntriesForTireCode;
using MediatR;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.TasyDbMatcher.Messaging.Procedures;

public class PerformTireDbMatchingProcedure : IProcedure
{
    public string Id => "PerformTireDbMatching";
    public int SchemaVersion => 1;

    public IApElement InputSchema => new ApObject(
        properties: new()
        {
            { "rawCode", ApString.Template() },
            { "postprocessedTireCode", ApString.Template() },
            { "vehicleClass", ApString.Template() },
            { "width", ApDecimal.Template() },
            { "aspectRatio", ApDecimal.Template() },
            { "construction", ApString.Template() },
            { "diameter", ApDecimal.Template() },
            { "loadIndex", ApString.Template() },
            { "speedRating", ApString.Template() }
        },
        nonRequiredProperties:
        [
            "vehicleClass", "width", "aspectRatio", "construction", "diameter", "loadIndex", "speedRating"
        ]
    );

    public IApElement OutputSchema => ApList.Template(
        new ApObject(
            properties: new()
            {
                {
                    "entry", new ApObject(
                        properties: new()
                        {
                            { "width", ApInt.Template() },
                            { "diameter", ApDecimal.Template() },
                            { "profile", ApDecimal.Template() },
                            { "construction", ApString.Template() },
                            { "loadIndex", ApInt.Template() },
                            { "speedIndex", ApString.Template() },
                            { "lisi", ApString.Template() },
                        },
                        nonRequiredProperties: ["loadIndex", "speedIndex", "lisi"]
                    )
                },
                { "requiredCharEdits", ApInt.Template() },
                { "estimatedAccuracy", ApDecimal.Template() },
            }
        )
    );

    private readonly IMediator _mediator;

    public PerformTireDbMatchingProcedure(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<DataResult<IApElement>> ExecuteAsync(RunPipelineStep step)
    {
        var parsedInputResult = ParseTireCodeFromInput(step.CurrentStepInput);
        if (parsedInputResult.IsFailure)
            return DataResult<IApElement>.Failure(parsedInputResult.Failures);

        var query = new GetTasyDbEntriesForTireCodeQuery(
            DetectedCode: parsedInputResult.Data!,
            MaxEntries: null
        );
        var result = await _mediator.Send(query);

        return result.Map(
            onSuccess: entries =>
            {
                var mappedEntries = entries
                    .Select<TireDbMatchDto, IApElement>(e =>
                    {
                        var entryObject = new ApObject(
                            properties: new()
                            {
                                { "width", new ApInt(e.TireEntry.Width) },
                                { "diameter", new ApDecimal(e.TireEntry.Diameter) },
                                { "profile", new ApDecimal(e.TireEntry.Profile) },
                                { "construction", new ApString(e.TireEntry.Construction) }
                            }
                        );
                        AddNonRequiredPropertyIfNotNull(entryObject, e.TireEntry.LoadIndex, "loadIndex",
                            li => new ApInt(li!.Value));
                        AddNonRequiredPropertyIfNotNull(entryObject, e.TireEntry.SpeedIndex, "speedIndex",
                            si => new ApString(si));
                        AddNonRequiredPropertyIfNotNull(entryObject, e.TireEntry.LoadIndexSpeedIndex, "lisi",
                            lisi => new ApString(lisi));
                        return new ApObject(
                            properties: new()
                            {
                                {
                                    "entry", entryObject
                                },
                                { "requiredCharEdits", new ApInt(e.RequiredCharEdits) },
                                { "estimatedAccuracy", new ApDecimal(e.EstimatedAccuracy) },
                            }
                        );
                    })
                    .ToList();

                return DataResult<IApElement>.Success(new ApList(mappedEntries));
            },
            onFailure: DataResult<IApElement>.Failure
        );
    }

    private DataResult<DetectedTireCodeDto> ParseTireCodeFromInput(IApElement input)
    {
        Func<string, DataResult<DetectedTireCodeDto>> getInputNotFoundResult = (key) =>
            DataResult<DetectedTireCodeDto>.Invalid(
                $"Input of procedure '{nameof(PerformTireDbMatchingProcedure)}' is missing required property '{key}'.");
        if (input is ApObject apObject)
        {
            var rawCode = apObject.GetPropertyOfTypeOrNull<ApString>("rawCode");
            if (rawCode is null)
                return getInputNotFoundResult("rawCode");
            var postprocessedTireCode = apObject.GetPropertyOfTypeOrNull<ApString>("postprocessedTireCode");
            if (postprocessedTireCode is null)
                return getInputNotFoundResult("postprocessedTireCode");

            var vehicleClass = apObject.GetPropertyOfTypeOrNull<ApString>("vehicleClass");
            var width = apObject.GetPropertyOfTypeOrNull<ApDecimal>("width");
            var aspectRatio = apObject.GetPropertyOfTypeOrNull<ApDecimal>("aspectRatio");
            var construction = apObject.GetPropertyOfTypeOrNull<ApString>("construction");
            var diameter = apObject.GetPropertyOfTypeOrNull<ApDecimal>("diameter");
            var loadIndex = apObject.GetPropertyOfTypeOrNull<ApString>("loadIndex");
            var speedRating = apObject.GetPropertyOfTypeOrNull<ApString>("speedRating");

            var dto = new DetectedTireCodeDto(
                rawCode.Value,
                postprocessedTireCode.Value,
                vehicleClass?.Value,
                width?.Value,
                aspectRatio?.Value,
                construction?.Value,
                diameter?.Value,
                loadIndex?.Value,
                speedRating?.Value
            );

            return DataResult<DetectedTireCodeDto>.Success(dto);
        }

        return DataResult<DetectedTireCodeDto>
            .Invalid($"Input doesn't match '{nameof(PerformTireDbMatchingProcedure)}' procedure input schema");
    }

    private void AddNonRequiredPropertyIfNotNull<T>(ApObject apObject, T? property, string key,
        Func<T, IApElement> createOnNotNull)
    {
        if (property is null)
            return;
        var apElement = createOnNotNull(property);
        apObject.Properties.Add(key, apElement);
        apObject.NonRequiredProperties.Add(key);
    }
}