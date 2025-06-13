using AiPipeline.Orchestration.Contracts.Events.NodeAdvertisement;
using AiPipeline.Orchestration.Contracts.Schema.Properties;

namespace AiPipeline.TireOcr.Ocr.Messaging.Constants;

public static class MessagingConstants
{
    public static string NodeQueueName => "tire-ocr-ocr";
    public static string AdvertisementsExchangeName => "node-advertisements";
    public static string AdvertisementsQueueName => "node-advertisements-queue";

    public static List<ProcedureDescriptor> AvailableProcedures =
    [
        new()
        {
            Name = "PerformSingleOcr",
            Input = new ApFile("", ""),
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
    ];
}