using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Data;
using Shahbazi.Store.Enums;
using Shahbazi.Store.Models;

namespace Shahbazi.Store.Common.Auth;

// Creates the first Admin from configuration (SeedAdmin section), only if it does not exist yet.
public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        var phoneNumber = configuration["SeedAdmin:PhoneNumber"];
        var password = configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var context = services.GetRequiredService<AppDbContext>();

        if (await context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber))
        {
            return;
        }

        var hasher = services.GetRequiredService<IPasswordHasher<User>>();
        var fullName = configuration["SeedAdmin:FullName"] ?? "Admin";

        var admin = new User(fullName, phoneNumber, "N/A", UserRole.Admin);
        admin.SetPasswordHash(hasher.HashPassword(admin, password));

        context.Users.Add(admin);
        await context.SaveChangesAsync();
    }
}