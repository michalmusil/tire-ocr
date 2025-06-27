namespace TireOcr.Shared.Result;

public record Failure(int Code, string Message)
{
    public void ThrowAsException(Exception? innerException = null) =>
        throw new ResultFailureException(this, innerException);
}