namespace Shahbazi.Store.Common.ResultPattern;

public class Result
{
    public enum ApiMessageType
    {
        Success,
        BadRequest,
        NotFound,
        Conflict,
        Unauthorized
    }

    public sealed record ApiMessage(string Message, ApiMessageType Type);

    public ApiMessage Message { get; }
    public bool IsSuccess => Message.Type == ApiMessageType.Success;

    protected Result(ApiMessage message)
    {
        Message = message;
    }

    public static Result Success(string? message = null) => new(new(message, ApiMessageType.Success));

    public static Result NotFound(string entity) => NotFoundError($"{entity} not found.");
    public static Result InvalidId(string entity) => BadRequestError($"{entity} id is invalid.");

    public static Result NotFoundError(string message) => new(new(message, ApiMessageType.NotFound));
    public static Result ConflictError(string message) => new(new(message, ApiMessageType.Conflict));
    public static Result BadRequestError(string message) => new(new(message, ApiMessageType.BadRequest));
    public static Result UnauthorizedError(string message) => new(new(message, ApiMessageType.Unauthorized));
}

public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(T? value, ApiMessage message) : base(message)
    {
        Value = value;
    }

    public static Result<T> Success(T value, string? message = null) => new(value, new(message, ApiMessageType.Success));

    public static Result<T> Failure(ApiMessage message) => new(default, message);

    public static new Result<T> NotFound(string entity) => NotFoundError($"{entity} not found.");
    public static new Result<T> InvalidId(string entity) => BadRequestError($"{entity} id is invalid.");

    public static new Result<T> NotFoundError(string message) => new(default, new(message, ApiMessageType.NotFound));
    public static new Result<T> ConflictError(string message) => new(default, new(message, ApiMessageType.Conflict));
    public static new Result<T> BadRequestError(string message) => new(default, new(message, ApiMessageType.BadRequest));
    public static new Result<T> UnauthorizedError(string message) => new(default, new(message, ApiMessageType.Unauthorized));
}