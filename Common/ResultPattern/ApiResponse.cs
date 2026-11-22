namespace Shahbazi.Store.Common.ResultPattern;

public record ApiResponse(string Message);

public record ApiResponse<T>(string Message, T? Data);