using Grpc.Core;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.FileService.GrpcServer.Extensions;

public static class ResultExtension
{
    public static RpcException ToRpcException<T>(this DataResult<T> result)
    {
        var primaryFailure = result.PrimaryFailure!;
        var statusCode = GetStatusCode(primaryFailure.Code);
        return new RpcException(new Status(statusCode, primaryFailure.Message));
    }

    public static RpcException ToRpcException<T>(this Result result)
    {
        var primaryFailure = result.PrimaryFailure!;
        var statusCode = GetStatusCode(primaryFailure.Code);
        return new RpcException(new Status(statusCode, primaryFailure.Message));
    }

    private static StatusCode GetStatusCode(int code)
    {
        return code switch
        {
            401 => StatusCode.Unauthenticated,
            403 => StatusCode.PermissionDenied,
            404 => StatusCode.NotFound,
            408 => StatusCode.ResourceExhausted,
            409 => StatusCode.AlreadyExists,
            422 => StatusCode.InvalidArgument,
            499 => StatusCode.Cancelled,
            _ => StatusCode.Internal
        };
    }
}