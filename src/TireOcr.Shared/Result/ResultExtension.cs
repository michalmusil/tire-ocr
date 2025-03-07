namespace TireOcr.Shared.Result;

public static class ResultExtension
{
    public static bool IsUnauthorized(this TireOcr.Shared.Result.Result result) => result is { IsSuccess: false, PrimaryFailure.Code: 401 };
    public static bool IsForbidden(this TireOcr.Shared.Result.Result result) => result is { IsSuccess: false, PrimaryFailure.Code: 403 };
    public static bool IsNotFound(this TireOcr.Shared.Result.Result result) => result is { IsSuccess: false, PrimaryFailure.Code: 404 };
    public static bool IsConflict(this TireOcr.Shared.Result.Result result) => result is { IsSuccess: false, PrimaryFailure.Code: 409 };
    public static bool IsInvalid(this TireOcr.Shared.Result.Result result) => result is { IsSuccess: false, PrimaryFailure.Code: 422 };
}