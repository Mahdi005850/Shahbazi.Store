namespace Shahbazi.Store.Common.ResultPattern;

public class Error(Error.ErrorType type, string message)
{
    public enum ErrorType
    {
        None,
        BadRequest,
        NotFound,
        Conflict
    }

    public ErrorType Type { get; } = type;
    public string Message { get; } = message;
}