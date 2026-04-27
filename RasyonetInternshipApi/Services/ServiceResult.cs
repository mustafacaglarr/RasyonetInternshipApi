namespace RasyonetInternshipApi.Services;

public enum ServiceResultStatus
{
    Success,
    Invalid,
    NotFound,
    Conflict,
    ExternalApiError
}

public class ServiceResult<T>
{
    private ServiceResult(ServiceResultStatus status, T? value, string? errorMessage)
    {
        Status = status;
        Value = value;
        ErrorMessage = errorMessage;
    }

    public ServiceResultStatus Status { get; }

    public T? Value { get; }

    public string? ErrorMessage { get; }

    public static ServiceResult<T> Success(T value) => new(ServiceResultStatus.Success, value, null);

    public static ServiceResult<T> Invalid(string message) => new(ServiceResultStatus.Invalid, default, message);

    public static ServiceResult<T> NotFound(string message) => new(ServiceResultStatus.NotFound, default, message);

    public static ServiceResult<T> Conflict(string message) => new(ServiceResultStatus.Conflict, default, message);

    public static ServiceResult<T> ExternalApiError(string message) => new(ServiceResultStatus.ExternalApiError, default, message);
}
