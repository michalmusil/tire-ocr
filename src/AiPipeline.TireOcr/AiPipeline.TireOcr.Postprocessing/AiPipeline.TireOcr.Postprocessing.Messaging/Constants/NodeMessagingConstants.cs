using AiPipeline.Orchestration.Shared;
using AiPipeline.Orchestration.Shared.Contracts.Events.NodeAdvertisement;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Properties;

namespace AiPipeline.TireOcr.Postprocessing.Messaging.Constants;

public static class NodeMessagingConstants
{
    public static NodeAdvertised NodeAdvertisement = new()
    {
        NodeName = MessagingConstants.TireOcrPostprocessingQueueName,
        Procedures =
        [
            new()
            {
                Name = "PerformTireCodePostprocessing",
                Input = new ApString(""),
                Output = new ApObject(
                    properties: new()
                    {
                        { "RawCode", new ApString("") },
                        { "PostprocessedTireCode", new ApString("") },
                        { "VehicleClass", new ApString("") },
                        { "Width", new ApDecimal(0m) },
                        { "AspectRatio", new ApDecimal(0m) },
                        { "Construction", new ApString("") },
                        { "Diameter", new ApDecimal(0m) },
                        { "LoadIndex", new ApString("") },
                        { "SpeedRating", new ApString("") }
                    },
                    nonRequiredProperties:
                    [
                        "VehicleClass", "Width", "AspectRatio", "Construction", "Diameter", "LoadIndex", "SpeedRating"
                    ])
            }
        ]
    };
}