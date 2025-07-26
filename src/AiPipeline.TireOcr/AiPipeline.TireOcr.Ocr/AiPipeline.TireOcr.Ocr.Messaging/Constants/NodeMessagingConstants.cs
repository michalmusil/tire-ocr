using AiPipeline.Orchestration.Shared.All.Constants;
using AiPipeline.Orchestration.Shared.All.Contracts.Events.NodeAdvertisement;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;

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
                        { "detectorType", ApString.Template() },
                        { "image", ApFile.Template(["image/jpeg", "image/png", "image/webp"]) }
                    }
                ),
                Output = new ApString("")
            }
        ]
    };
}