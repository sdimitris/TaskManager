namespace TaskManager.Domain.Common.Enums;

public enum KnownApplicationErrorEnum
{
    None = 1,
    GenericError,
    SqlGenericError,
    CatsApiError,
    CatNotFound
}