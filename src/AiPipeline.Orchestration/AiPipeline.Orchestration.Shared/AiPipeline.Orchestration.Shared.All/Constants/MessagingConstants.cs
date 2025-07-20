namespace AiPipeline.Orchestration.Shared.All.Constants;

public static class MessagingConstants
{
    public static string TireOcrPreprocessingQueueName => "tire-ocr-preprocessing";
    public static string TireOcrOcrQueueName => "tire-ocr-ocr";
    public static string TireOcrPostprocessingQueueName => "tire-ocr-postprocessing";

    public static string RunPipelineExchangeName => "run-pipeline";
    public static string CompletedPipelineStepsExchangeName => "completed-pipeline-steps";
    public static string CompletedPipelineStepsQueueName => "completed-pipeline-steps-queue";

    public static string CompletedPipelinesExchangeName => "completed-pipelines";
    public static string CompletedPipelinesQueueName => "completed-pipelines-queue";


    public static string FailedPipelinesExchangeName => "failed-pipelines";
    public static string FailedPipelinesQueueName => "failed-pipelines-queue";

    public static string AdvertisementsExchangeName => "node-advertisements";
    public static string AdvertisementsQueueName => "node-advertisements-queue";
}