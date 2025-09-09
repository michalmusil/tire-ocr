namespace AiPipeline.Orchestration.FileService.GrpcSdk.Constants;

public static class GrpcSdkConstants
{
    private const int Mb = 1024 * 1024;
    public static readonly int MaxReceiveMessageSize = 300 * Mb;
    public static readonly int MaxSendMessageSize = 300 * Mb;
}