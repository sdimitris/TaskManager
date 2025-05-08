using TaskManager.Domain.Common.Enums;

namespace TaskManager.Domain.Common.Result;

public class Result<TValue>
{

    public TValue Value
    {
        get => !IsSuccess ? throw new InvalidOperationException("Outcome is in failed state - cannot get return value.") : _value!;
        protected set => _value = value;
    }
    
    private TValue? _value;

    public bool IsSuccess { get; set; }
    
    public bool IsFailure => !IsSuccess;
    public Error Error { get; set; }
    
    private Result(bool isSuccess, TValue? value, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error ?? Error.New(string.Empty, null, KnownApplicationErrorEnum.None);
        
        if (value != null)
            Value = value;
    }
    
    public static Result<TValue> Ok(TValue? value) => new(true, value, null);
    public static Result<TValue> Failure(Error error) => new(false, default, error);
    public static Result<TValue> FromFailure<TSourceResultValue>(Result<TSourceResultValue> result) => result.IsSuccess ? throw new InvalidOperationException("Expecting a failed result.") : new(false, default, result.Error);
}