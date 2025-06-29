using AiPipeline.Orchestration.Shared.Constants;
using AiPipeline.Orchestration.Shared.Contracts.Events.NodeAdvertisement;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Properties;

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
                Input = new ApFile(Guid.Empty, "", supportedContentTypes: ["image/jpeg", "image/png", "image/webp"]),
                Output = new ApString("")
            }
        ]
    };
}