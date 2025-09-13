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

        // создадим первого админа, если нет
        var email = "admin@event.local";
        var admin = await users.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
            // пароль только для dev
            var result = await users.CreateAsync(admin, "Admin!123");
            if (result.Succeeded)
                await users.AddToRoleAsync(admin, adminRole);
        }
    }
}