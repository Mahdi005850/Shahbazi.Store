namespace Shahbazi.Store.Common.CodeResponse;

public record ApiResponse(string Message);

public record ApiResponse<T>(string Message, T? Data);

public sealed record ApiMessages(string? Success = null, string? BadRequest = null, string? NotFound = null, string? Conflict = null);