namespace TaskManager.Domain.Common.Enums;

public enum KnownApplicationErrorEnum
{
    None = 1,
    GenericError,
    SqlGenericError,
    TaskNotFound,
    UserNotFound,
    UserAlreadyExist
}