namespace AiPipeline.Orchestration.Shared.Constants;

public static class MessagingConstants
{
    public static string TireOcrPreprocessingQueueName => "tire-ocr-preprocessing";
    public static string TireOcrOcrQueueName => "tire-ocr-ocr";
    public static string TireOcrPostprocessingQueueName => "tire-ocr-postprocessing";

    public static string RunPipelineExchangeName => "run-pipeline";

    public static string AdvertisementsExchangeName => "node-advertisements";
    public static string AdvertisementsQueueName => "node-advertisements-queue";
}