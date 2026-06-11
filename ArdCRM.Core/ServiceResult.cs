namespace ArdCRM.Core;

public class ServiceResult
{
    public bool Success { get; protected set; }
    public string Message { get; protected set; } = string.Empty;
    public List<string> Errors { get; protected set; } = new();

    public static ServiceResult Ok(string message = "") =>
        new() { Success = true, Message = message };

    public static ServiceResult Fail(string error) =>
        new() { Success = false, Message = error, Errors = new List<string> { error } };

    public static ServiceResult Fail(List<string> errors) =>
        new() { Success = false, Message = errors.FirstOrDefault() ?? "Hata oluştu.", Errors = errors };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; private set; }

    public static ServiceResult<T> Ok(T data, string message = "") =>
        new() { Success = true, Data = data, Message = message };

    public static new ServiceResult<T> Fail(string error) =>
        new() { Success = false, Message = error, Errors = new List<string> { error } };

    public static new ServiceResult<T> Fail(List<string> errors) =>
        new() { Success = false, Message = errors.FirstOrDefault() ?? "Hata oluştu.", Errors = errors };
}
