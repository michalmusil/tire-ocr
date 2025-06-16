using AiPipeline.Orchestration.Shared;
using AiPipeline.Orchestration.Shared.Contracts.Events.NodeAdvertisement;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Properties;

namespace AiPipeline.TireOcr.Ocr.Messaging.Constants;

public static class NodeMessagingConstants
{
    public static NodeAdvertised NodeAdvertisement = new()
    {
        NodeName = MessagingConstants.TireOcrOcrQueueName,
        Procedures =
        [
            new()
            {
                Name = "PerformSingleOcr",
                Input = new ApFile("", "", ""),
                Output = new ApObject(new()
                {
                    { "DetectedCode", new ApString("") },
                    {
                        "Billing", new ApObject(new()
                        {
                            { "InputAmount", new ApDecimal(0) },
                            { "OutputAmount", new ApDecimal(0) },
                            { "Unit", new ApString("") }
                        })
                    }
                })
            }
        ]
    };
}