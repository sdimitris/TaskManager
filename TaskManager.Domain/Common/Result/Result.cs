namespace TaskManager.Domain.Common.Result;

public class Result
{
    public bool IsSuccess { get; set; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; set; }

    private Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Ok() => new(true, Error.New(string.Empty, null));
    public static Result Failure(Error error) => new(false, error);
    public static Result FromFailure(Result result) => result.IsSuccess ? throw new InvalidOperationException("Expecting a failed outcome.") : new(false, result.Error);
    public static Result FromFailure<TSourceResultValue>(Result<TSourceResultValue> result) => result.IsSuccess ? throw new InvalidOperationException("Expecting a failed outcome.") : new(false, result.Error);

}