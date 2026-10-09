namespace Shahbazi.Store.Common.ResultPattern;

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public ResultErrorType ErrorType { get; }
    protected Result(bool isSuccess, string? error, ResultErrorType errorType)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorType = errorType;
    }
    public static Result Success()
    {
        return new Result(true, null, ResultErrorType.None);
    }
    public static Result Failure(string error, ResultErrorType errortype)
    {
        return new Result(false, error, errortype);
    }
}
public class Result<T> : Result
{
    public T? Value { get; }
    private Result(bool isSuccess, T? value, string? error, ResultErrorType errorType) : base(isSuccess, error, errorType)
    {
        Value = value;
    }
    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, null, ResultErrorType.None);
    }
    public static Result<T> Failure(string error, ResultErrorType errorType)
    {
        return new Result<T>(false, default, error, errorType);
    }
}