using Shahbazi.Store.Enums;

namespace Shahbazi.Store.Common.Auth;

public static class Roles
{
    public const string Buyer = nameof(UserRole.Buyer);
    public const string Seller = nameof(UserRole.Seller);
    public const string Admin = nameof(UserRole.Admin);
}

public static class Policies
{
    public const string AdminOnly = "AdminOnly";
    public const string SellerOrAdmin = "SellerOrAdmin";
    public const string BuyerOnly = "BuyerOnly";
}