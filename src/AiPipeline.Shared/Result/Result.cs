namespace TireOcr.Shared.Result;

public class Result
{
    public bool IsSuccess { get; protected set; }
    public bool IsFailure => !IsSuccess;
    public Failure[] Failures { get; protected set; }

    public Failure? PrimaryFailure => Failures.FirstOrDefault();

    protected Result(bool isSuccess, Failure[] failures)
    {
        IsSuccess = isSuccess;
        Failures = failures;
    }

    public static Result Success() => new(true, []);

    public static Result Failure(Failure failure) => new Result(false, [failure]);

    public static Result Failure(params Failure[] failures) => new Result(false, failures);

    public static Result Unauthorized(string message, params Failure[] failures) =>
        Failure(new[] { new Failure(401, message) }.Concat(failures).ToArray());

    public static Result Forbidden(string message, params Failure[] failures) =>
        Failure(new[] { new Failure(403, message) }.Concat(failures).ToArray());

    public static Result NotFound(string message, params Failure[] failures) =>
        Failure(new[] { new Failure(404, message) }.Concat(failures).ToArray());

    public static Result Timeout(string message, params Failure[] failures) =>
        Failure(new[] { new Failure(408, message) }.Concat(failures).ToArray());

    public static Result Conflict(string message, params Failure[] failures) =>
        Failure(new[] { new Failure(409, message) }.Concat(failures).ToArray());

    public static Result Invalid(string message, params Failure[] failures) =>
        Failure(new[] { new Failure(422, message) }.Concat(failures).ToArray());

    public static Result Cancelled(string message, params Failure[] failures) =>
        Failure(new[] { new Failure(499, message) }.Concat(failures).ToArray());

    public FailureMessages GetMessages()
    {
        var messages = Failures.Select(f => f.Message);
        return new FailureMessages(messages);
    }

    public void Fold(Action onSuccess, Action<Failure[]> onFailure)
    {
        if (IsSuccess)
        {
            onSuccess();
            return;
        }

        onFailure(Failures);
    }

    public async Task FoldAsync(Func<Task> onSuccess, Func<Failure[], Task> onFailure)
    {
        if (IsSuccess)
        {
            await onSuccess();
            return;
        }

        await onFailure(Failures);
    }

    public TResult Map<TResult>(Func<TResult> onSuccess, Func<Failure[], TResult> onFailure)
    {
        if (IsSuccess)
        {
            return onSuccess();
        }

        return onFailure(Failures);
    }

    public Task<TResult> MapAsync<TResult>(Func<Task<TResult>> onSuccess, Func<Failure[], Task<TResult>> onFailure)
    {
        if (IsSuccess)
        {
            return onSuccess();
        }

        return onFailure(Failures);
    }
}