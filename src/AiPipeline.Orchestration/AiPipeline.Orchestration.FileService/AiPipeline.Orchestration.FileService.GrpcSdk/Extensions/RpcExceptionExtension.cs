using Grpc.Core;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.FileService.GrpcSdk.Extensions;

public static class RpcExceptionExtension
{
    public static DataResult<T> ToDataResult<T>(this RpcException ex)
    {
        var failure = GetFailure(ex);
        return DataResult<T>.Failure(failure);
    }

    public static Result ToResult(this RpcException ex)
    {
        var failure = GetFailure(ex);
        return Result.Failure(failure);
    }

    private static Failure GetFailure(RpcException ex)
    {
        return ex.StatusCode switch
        {
            StatusCode.InvalidArgument => new Failure(422, "Grpc failure: Invalid argument"),
            StatusCode.NotFound => new Failure(404, "Grpc failure: Not found"),
            StatusCode.AlreadyExists => new Failure(409, "Grpc failure: Already exists"),
            StatusCode.PermissionDenied => new Failure(403, "Grpc failure: Permission denied"),
            StatusCode.Unauthenticated => new Failure(401, "Grpc failure: Unauthenticated"),
            StatusCode.ResourceExhausted => new Failure(408, "Grpc failure: Resource exhausted"),
            StatusCode.Cancelled => new Failure(499, "Grpc failure: Cancelled"),
            _ => new Failure(500, "Grpc failure: Internal error"),
        };
    }
}