namespace Shahbazi.Store.Common.ResultPattern;

public static class ResultExtensions
{
    // Converts the failure of one Result into a Result<T> of another type (keeps error type and message).
    public static Result<T> ToFailure<T>(this Result source)
    {
        var error = source.Error!;

        return error.Type switch
        {
            Error.ErrorType.NotFound => Result<T>.NotFoundError(error.Message),
            Error.ErrorType.Conflict => Result<T>.ConflictError(error.Message),
            _ => Result<T>.BadRequestError(error.Message)
        };
    }
}