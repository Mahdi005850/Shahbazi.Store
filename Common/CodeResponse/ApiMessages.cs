namespace Shahbazi.Store.Common.CodeResponse;

public record ApiResponse(int Code, string Message);

public record ApiResponse<T>(int Code, string Message, T? Data);

public sealed record ApiMessages(string? Success = null, string? BadRequest = null, string? NotFound = null, string? Conflict = null);