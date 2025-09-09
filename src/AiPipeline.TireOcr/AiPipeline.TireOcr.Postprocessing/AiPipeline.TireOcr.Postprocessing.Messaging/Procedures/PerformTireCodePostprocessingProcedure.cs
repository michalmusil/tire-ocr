using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;
using AiPipeline.Orchestration.Shared.Nodes.Procedures;
using MediatR;
using TireOcr.Postprocessing.Application.Queries.TireCodePostprocessing;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.Postprocessing.Messaging.Procedures;

public class PerformTireCodePostprocessingProcedure : IProcedure
{
    public string Id => "PerformTireCodePostprocessing";
    public int SchemaVersion => 1;
    public IApElement InputSchema => ApString.Template();

    public IApElement OutputSchema => new ApObject(
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

    private readonly IMediator _mediator;

    public PerformTireCodePostprocessingProcedure(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<DataResult<IApElement>> ExecuteAsync(RunPipelineStep step)
    {
        var input = step.CurrentStepInput;

        var schemaCompatible = InputSchema.HasCompatibleSchemaWith(input);
        if (!schemaCompatible)
            return DataResult<IApElement>.Invalid(
                $"Procedure '{nameof(PerformTireCodePostprocessingProcedure)}' failed: input schema is not compatible");

        var inputString = (ApString)input;
        var rawTireCode = inputString.Value;

        var query = new TireCodePostprocessingQuery(rawTireCode);
        var result = await _mediator.Send(query);

        return result.Map(
            onSuccess: data =>
            {
                var returnedObject = new ApObject(
                    properties: new()
                    {
                        { "rawCode", new ApString(data.RawCode) },
                        { "postprocessedTireCode", new ApString(data.PostprocessedTireCode) }
                    }
                );
                AddPropIfNotNull(returnedObject, data.VehicleClass, "vehicleClass", vc => new ApString(vc));
                AddPropIfNotNull(returnedObject, data.Width, "width", w => new ApDecimal(w!.Value));
                AddPropIfNotNull(returnedObject, data.AspectRatio, "aspectRatio", ar => new ApDecimal(ar!.Value));
                AddPropIfNotNull(returnedObject, data.Construction, "construction", c => new ApString(c));
                AddPropIfNotNull(returnedObject, data.Diameter, "diameter", d => new ApDecimal(d!.Value));
                AddPropIfNotNull(returnedObject, data.LoadIndex, "loadIndex", li => new ApString(li));
                AddPropIfNotNull(returnedObject, data.SpeedRating, "speedRating", sr => new ApString(sr));

                return DataResult<IApElement>.Success(returnedObject);
            },
            onFailure: DataResult<IApElement>.Failure
        );
    }

    private void AddPropIfNotNull<T>(ApObject apObject, T? property, string key, Func<T, IApElement> creteOnNotNull)
    {
        if (property is null)
            return;
        var apElement = creteOnNotNull(property);
        apObject.Properties.Add(key, apElement);
        apObject.NonRequiredProperties.Add(key);
    }
}