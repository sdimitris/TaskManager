using TaskManager.Domain.Common.Enums;

namespace TaskManager.Domain.Common.Result;

public class Error
{
    public string Message { get; set; }
    public KnownApplicationErrorEnum ApplicationError { get; set; }
    public Exception? Exception { get; set; }

    private Error(string message, KnownApplicationErrorEnum applicationError = KnownApplicationErrorEnum.GenericError,
        Exception? exception = null)
    {
        ApplicationError = applicationError;
        Message = message;
        Exception = exception;
    }
    
    public static Error New(string message, Exception? ex, KnownApplicationErrorEnum errorEnum = KnownApplicationErrorEnum.GenericError) => new(message, errorEnum, ex);

    public string GetError()
    {
        return $"ApplicationError: {Message} | error code: {ApplicationError} | ExceptionMessage: {Exception?.Message}";
    }
}