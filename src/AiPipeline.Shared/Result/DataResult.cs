namespace TireOcr.Shared.Result;

public class DataResult<T> : Result
{
    public T? Data { get; }

    private DataResult(T? data, Failure[] failures) : base(data != null, failures)
    {
        Data = data;
    }

    public static DataResult<T> Success(T value) => new(value, []);

    public new static DataResult<T> Failure(Failure failure) => new(default, [failure]);

    public new static DataResult<T> Failure(params Failure[] failures) => new(default, failures);
    
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

    public new static DataResult<T> Conflict(string message, params Failure[] failures) =>
        Failure(new[] { new Failure(409, message) }.Concat(failures).ToArray());

    public new static DataResult<T> Invalid(string message, params Failure[] failures) =>
        Failure(new[] { new Failure(422, message) }.Concat(failures).ToArray());
}