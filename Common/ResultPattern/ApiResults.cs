using static Shahbazi.Store.Common.ResultPattern.Result;

namespace Shahbazi.Store.Common.ResultPattern;

public static class ApiResults
{
    public static IResult ToHttpResult<T>(Result<T> result)
    {
        if (!result.IsSuccess)
        {
            return ToErrorResult(result.Message);
        }

        return TypedResults.Ok(new ApiResponse<T>(result.Message.Message, result.Value));
    }

    public static IResult ToHttpResult(Result result)
    {
        if (!result.IsSuccess)
        {
            return ToErrorResult(result.Message);
        }

        return TypedResults.Ok(new ApiResponse(result.Message.Message));
    }

    public static IResult ToCreatedResult<T>(Result<T> result, Func<T, string> location)
    {
        if (!result.IsSuccess)
        {
            return ToErrorResult(result.Message);
        }

        var body = new ApiResponse<T>(result.Message.Message, result.Value);

        return TypedResults.Created(location(result.Value!), body);
    }

    private static IResult ToErrorResult(ApiMessage message)
    {
        var body = new ApiResponse(message.Message);

        return message.Type switch
        {
            ApiMessageType.BadRequest => TypedResults.BadRequest(body),
            ApiMessageType.NotFound => TypedResults.NotFound(body),
            ApiMessageType.Conflict => TypedResults.Conflict(body),
            _ => TypedResults.Problem(message.Message)
        };
    }
}