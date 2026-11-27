namespace Shahbazi.Store.DTOs.Responses;

public record LoginResponse(string AccessToken, DateTime ExpiresAtUtc, UserGetResponse User);