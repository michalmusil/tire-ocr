using AiPipeline.Orchestration.Shared.Constants;
using AiPipeline.Orchestration.Shared.Contracts.Events.NodeAdvertisement;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Properties;

namespace AiPipeline.TireOcr.Ocr.Messaging.Constants;

public static class NodeMessagingConstants
{
    public static string PerformSingleOcrProcedureName = "PerformSingleOcr";

    public static NodeAdvertised NodeAdvertisement = new()
    {
        NodeName = MessagingConstants.TireOcrOcrQueueName,
        Procedures =
        [
            new()
            {
                Name = "PerformSingleOcr",
                Input = new ApFile("", "", ""),
                Output = new ApString("")
            }
        ]
    };
}