using AiPipeline.Orchestration.Shared.All.Constants;
using AiPipeline.Orchestration.Shared.All.Contracts.Events.NodeAdvertisement;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;
using TireOcr.Ocr.Domain;

namespace AiPipeline.TireOcr.Ocr.Messaging.Constants;

public static class NodeMessagingConstants
{
    public static readonly string PerformSingleOcrProcedureId = "PerformSingleOcr";

    public static NodeAdvertised NodeAdvertisement = new()
    {
        NodeId = MessagingConstants.TireOcrOcrQueueName,
        Procedures =
        [
            new()
            {
                Id = PerformSingleOcrProcedureId,
                Input = new ApObject(
                    properties: new()
                    {
                        {
                            "detectorType", ApEnum.Template(
                                Enum.GetValues(typeof(TireCodeDetectorType))
                                    .Cast<TireCodeDetectorType>()
                                    .Select(t => t.ToString())
                                    .ToArray()
                            )
                        },
                        { "image", ApFile.Template(["image/jpeg", "image/png", "image/webp"]) }
                    }
                ),
                Output = new ApString("")
            }
        ]
    };
}