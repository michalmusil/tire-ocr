namespace TireOcr.Shared.Result;

public class ResultFailureException : Exception
{
    public int Code { get; }

    public ResultFailureException(Failure failure, Exception? innerException = null)
        : base(failure.Message, innerException)
    {
        Code = failure.Code;
    }
}