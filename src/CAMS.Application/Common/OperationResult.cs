namespace CAMS.Application.Common;

/// <summary>
/// Represents the result of an operation.
/// It shows if the operation was successful, holds the result data,
/// and lists any error messages.
/// </summary>
/// <typeparam name="T">The type of data returned when the operation is successful.</typeparam>

public class OperationResult<T>
{
    public bool IsSuccess { get; }
    public T Data { get; }
    public IEnumerable<string> Errors { get; }

    private OperationResult(bool isSuccess, T data, IEnumerable<string> errors)
    {
        IsSuccess = isSuccess;
        Data = data;
        Errors = errors;
    }

    public static OperationResult<T> Success(T data) => new OperationResult<T>(true, data, Enumerable.Empty<string>());
    public static OperationResult<T> Fail(FluentValidation.Results.ValidationResult validationResult) 
        => new OperationResult<T>(false, default, validationResult.Errors.Select(e => e.ErrorMessage));
}

