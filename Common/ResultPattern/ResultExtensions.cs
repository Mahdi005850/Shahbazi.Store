namespace Shahbazi.Store.Common.ResultPattern;

public static class ResultExtensions
{
    // Converts the failure of one Result into a Result<T> of another type (keeps message and type).
    public static Result<T> ToFailure<T>(this Result source) => Result<T>.Failure(source.Message);
}