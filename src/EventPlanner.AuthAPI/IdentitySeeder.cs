using EventPlanner.AuthAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace EventPlanner.AuthAPI;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        const string adminRole = "Admin";
        if (!await roles.RoleExistsAsync(adminRole))
            await roles.CreateAsync(new ApplicationRole { Name = adminRole });
    }
}