using TaskManager.Domain.Common.Enums;

namespace TaskManager.Domain.Common.Result;

public class Error
{
    public int ErrorCode { get; set; } = 400;
    public string Message { get; set; }
    public KnownApplicationErrorEnum ApplicationError { get; set; }
    public Exception? Exception { get; set; }

    private Error(string message, KnownApplicationErrorEnum applicationError = KnownApplicationErrorEnum.GenericError, int errorCode = 400,
        Exception? exception = null)
    {
        ApplicationError = applicationError;
        Message = message;
        Exception = exception;
    }
    
    public static Error New(string message, Exception? ex, KnownApplicationErrorEnum errorEnum = KnownApplicationErrorEnum.GenericError, int errorCode = 400) => new(message, errorEnum, errorCode, ex);

    public string GetErrorInDetail()
    {
        return $"ApplicationError: {Message} | error code: {ApplicationError} | ExceptionMessage: {Exception?.Message}";
    }
    
    public string GetError()
    {
        return $"ApplicationError: {Message}";
    }
}