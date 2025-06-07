using Microsoft.AspNetCore.Mvc;

namespace TireOcr.Shared.Result;

public static class ResultExtension
{
    public static bool IsUnauthorized(this Result result) => result is { IsSuccess: false, PrimaryFailure.Code: 401 };
    public static bool IsForbidden(this Result result) => result is { IsSuccess: false, PrimaryFailure.Code: 403 };
    public static bool IsNotFound(this Result result) => result is { IsSuccess: false, PrimaryFailure.Code: 404 };
    public static bool IsConflict(this Result result) => result is { IsSuccess: false, PrimaryFailure.Code: 409 };
    public static bool IsInvalid(this Result result) => result is { IsSuccess: false, PrimaryFailure.Code: 422 };

    private static ActionResult FallbackResult => CreateActionResult(
        500,
        "Unexpected output",
        new Failure(500, "Server failed to asses output of your operation")
    );

    public static IActionResult ToActionResult(this Result result, Func<IActionResult> onSuccess)
    {
        if (result.IsSuccess)
            return onSuccess();

        var failures = result.Failures;
        var primaryFailure = failures.FirstOrDefault();
        var otherFailures = failures.Skip(1).ToArray();

        if (primaryFailure is null)
            return FallbackResult;

        return primaryFailure.ToActionResult(otherFailures);
    }

    public static ActionResult<TDestType> ToActionResult<TData, TDestType>(
        this DataResult<TData> result,
        Func<TData, ActionResult<TDestType>> onSuccess
    )
    {
        if (result.IsSuccess)
            return onSuccess(result.Data!);

        var failures = result.Failures;
        var primaryFailure = failures.FirstOrDefault();
        var otherFailures = failures.Skip(1).ToArray();

        if (primaryFailure is null)
            return FallbackResult;

        return primaryFailure.ToActionResult(otherFailures);
    }

    public static ActionResult ToActionResult(
        this Failure failure,
        params Failure[] otherFailures
    )
    {
        return failure.Code switch
        {
            401 => CreateActionResult(401, "Unauthorized", failure, otherFailures),
            403 => CreateActionResult(403, "Forbidden", failure, otherFailures),
            404 => CreateActionResult(404, "Not Found", failure, otherFailures),
            409 => CreateActionResult(409, "Conflict", failure, otherFailures),
            422 => CreateActionResult(422, "Invalid request data", failure, otherFailures),
            >= 400 and < 500 => CreateActionResult(400, "Bad request", failure, otherFailures),
            _ => CreateActionResult(500, "Unexpected result output", failure, otherFailures)
        };
    }

    private static ActionResult CreateActionResult(
        int statusCode,
        string title,
        Failure primaryFailure,
        params Failure[] otherFailures
    )
    {
        var details = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = primaryFailure.Message,
            // Extensions =
            // {
            //     { "Other failures", otherFailures.Select(f => f.Message) }
            // }
        };
        return new ObjectResult(details);
    }
}