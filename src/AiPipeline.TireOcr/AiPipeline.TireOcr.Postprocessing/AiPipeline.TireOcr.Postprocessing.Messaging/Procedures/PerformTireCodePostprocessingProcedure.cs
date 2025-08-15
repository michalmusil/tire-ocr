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
            { "RawCode", ApString.Template() },
            { "PostprocessedTireCode", ApString.Template() },
            { "VehicleClass", ApString.Template() },
            { "Width", ApDecimal.Template() },
            { "AspectRatio", ApDecimal.Template() },
            { "Construction", ApString.Template() },
            { "Diameter", ApDecimal.Template() },
            { "LoadIndex", ApString.Template() },
            { "SpeedRating", ApString.Template() }
        },
        nonRequiredProperties:
        [
            "VehicleClass", "Width", "AspectRatio", "Construction", "Diameter", "LoadIndex", "SpeedRating"
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
            onSuccess: data => DataResult<IApElement>.Success(
                new ApObject(
                    properties: new()
                    {
                        { "RawCode", new ApString(data.RawCode) },
                        { "PostprocessedTireCode", new ApString(data.PostprocessedTireCode) },
                        { "VehicleClass", new ApString(data.VehicleClass ?? "") },
                        { "Width", new ApDecimal(data.Width ?? default) },
                        { "AspectRatio", new ApDecimal(data.AspectRatio ?? default) },
                        { "Construction", new ApString(data.Construction ?? "") },
                        { "Diameter", new ApDecimal(data.Diameter ?? default) },
                        { "LoadIndex", new ApString(data.LoadIndex ?? "") },
                        { "SpeedRating", new ApString(data.SpeedRating ?? "") }
                    },
                    nonRequiredProperties:
                    [
                        "VehicleClass", "Width", "AspectRatio", "Construction", "Diameter", "LoadIndex", "SpeedRating"
                    ]
                )
            ),
            onFailure: DataResult<IApElement>.Failure
        );
    }
}