using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.Resources;

namespace Shahbazi.Store.Common.CodeResponse;

public static class ApiResults
{
    private static string DefaultSuccessMessage => Messages.Success;

    public static IResult ToHttpResult<T>(Result<T> result, ApiMessages? messages = null)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok(new ApiResponse<T>(messages?.Success ?? DefaultSuccessMessage, result.Value));
        }

        return ToErrorResult(result.Error!, messages);
    }

    public static IResult ToHttpResult(Result result, ApiMessages? messages = null)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok(new ApiResponse(messages?.Success ?? DefaultSuccessMessage));
        }

        return ToErrorResult(result.Error!, messages);
    }

    private static IResult ToErrorResult(Error error, ApiMessages? messages)
    {
        return error.Type switch
        {
            Error.ErrorType.BadRequest => TypedResults.BadRequest(new ApiResponse(messages?.BadRequest ?? error.Message)),
            Error.ErrorType.NotFound => TypedResults.NotFound(new ApiResponse(messages?.NotFound ?? error.Message)),
            Error.ErrorType.Conflict => TypedResults.Conflict(new ApiResponse(messages?.Conflict ?? error.Message)),
            _ => TypedResults.Problem(error.Message)
        };
    }
    public static IResult ToCreatedResult<T>(Result<T> result, Func<T, string> location, ApiMessages? messages = null)
    {
        if (!result.IsSuccess)
        {
            return ToErrorResult(result.Error!, messages);
        }

        var body = new ApiResponse<T>(messages?.Success ?? DefaultSuccessMessage, result.Value);

        return TypedResults.Created(location(result.Value!), body);
    }
}