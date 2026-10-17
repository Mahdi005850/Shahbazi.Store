namespace Shahbazi.Store.Common.ResultPattern;

public class Result
{
    public bool IsSuccess => Error is null;
    public Error? Error { get; }

    protected Result(Error? error = null)
    {
        Error = error;
    }

    public static Result Success() => new();

    public static Result NotFoundError(string message) => new(new(Error.ErrorType.NotFound, message));
    public static Result ConflictError(string message) => new(new(Error.ErrorType.Conflict, message));
    public static Result BadRequestError(string message) => new(new(Error.ErrorType.BadRequest, message));
}
public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(T? value, Error? error = null) : base(error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> NotFoundError(string message) => new(default, new(Error.ErrorType.NotFound, message));
    public static Result<T> ConflictError(string message) => new(default, new(Error.ErrorType.Conflict, message));
    public static Result<T> BadRequestError(string message) => new(default, new(Error.ErrorType.BadRequest, message));
}