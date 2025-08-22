namespace TireOcr.Shared.Result;

public class DataResult<T> : Result
{
    public T? Data { get; }

    private DataResult(bool success, T? data, Failure[] failures) : base(success, failures)
    {
        Data = data;
    }

    public static DataResult<T> Success(T value) => new(true, value, []);

    public new static DataResult<T> Failure(Failure failure) => new(false, default, [failure]);

    public new static DataResult<T> Failure(params Failure[] failures) => new(false, default, failures);

    public void Fold(Action<T> onSuccess, Action<Failure[]> onFailure)
    {
        if (IsSuccess && Data != null)
        {
            onSuccess(Data);
            return;
        }

        onFailure(Failures);
    }

    public TResult Map<TResult>(Func<T, TResult> onSuccess, Func<Failure[], TResult> onFailure)
    {
        if (IsSuccess && Data != null)
        {
            return onSuccess(Data);
        }

        return onFailure(Failures);
    }

    public Task<TResult> MapAsync<TResult>(Func<T, Task<TResult>> onSuccess, Func<Failure[], Task<TResult>> onFailure)
    {
        if (IsSuccess)
        {
            return onSuccess(Data!);
        }

        return onFailure(Failures);
    }

    public new static DataResult<T> Unauthorized(string message, params Failure[] failures) =>
        Failure(new[] { new Failure(401, message) }.Concat(failures).ToArray());

    public new static DataResult<T> Forbidden(string message, params Failure[] failures) =>
        Failure(new[] { new Failure(403, message) }.Concat(failures).ToArray());

    public new static DataResult<T> NotFound(string message, params Failure[] failures) =>
        Failure(new[] { new Failure(404, message) }.Concat(failures).ToArray());

    public new static DataResult<T> Timeout(string message, params Failure[] failures) =>
        Failure(new[] { new Failure(408, message) }.Concat(failures).ToArray());

    public new static DataResult<T> Conflict(string message, params Failure[] failures) =>
        Failure(new[] { new Failure(409, message) }.Concat(failures).ToArray());

    public new static DataResult<T> Invalid(string message, params Failure[] failures) =>
        Failure(new[] { new Failure(422, message) }.Concat(failures).ToArray());

    public new static DataResult<T> Cancelled(string message, params Failure[] failures) =>
        Failure(new[] { new Failure(499, message) }.Concat(failures).ToArray());
}