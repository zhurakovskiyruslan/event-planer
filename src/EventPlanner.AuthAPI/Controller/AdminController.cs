// AuthAPI/Controllers/AdminController.cs
using EventPlanner.AuthAPI.Data;
using EventPlanner.AuthAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.AuthAPI.Controller;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly RoleManager<ApplicationRole> _roles;
    private readonly AppIdentityDbContext _db;
    private readonly IWebHostEnvironment _env;

    public AdminController(
        UserManager<ApplicationUser> users,
        RoleManager<ApplicationRole> roles,
        AppIdentityDbContext db,
        IWebHostEnvironment env)
    {
        _users = users;
        _roles = roles;
        _db = db;
        _env = env;
    }

    // DTO для запроса
    public record SeedAdminDto(string Email, string Password, string Name);

    /// <summary>
    /// Создаёт администратора (dev-only). Идемпотентно.
    /// </summary>
    [HttpPost("seed")]
    [AllowAnonymous] // <-- оставляем анонимным, но ограничим окружением
    public async Task<IActionResult> Seed([FromBody] SeedAdminDto dto)
    {
        // Подстрахуемся: не даём запускать это в проде
        if (!_env.IsDevelopment())
            return Forbid("This endpoint is allowed only in Development.");

        // 1) Роль Admin
        if (!await _roles.RoleExistsAsync("Admin"))
        {
            var roleRes = await _roles.CreateAsync(new ApplicationRole { Name = "Admin" });
            if (!roleRes.Succeeded) return BadRequest(roleRes.Errors);
        }

        // 2) Пользователь
        var user = await _users.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, EmailConfirmed = true };
            var createRes = await _users.CreateAsync(user, dto.Password);
            if (!createRes.Succeeded) return BadRequest(createRes.Errors);

            // доменная запись Users (UserRead) с FK на AspNetUsers.Id
            _db.Set<UserRead>().Add(new UserRead
            {
                Name = dto.Name,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow,
                AppUserId = user.Id
            });
            await _db.SaveChangesAsync();
        }
        else
        {
            // если нашёлся — убедимся, что email подтверждён
            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                await _users.UpdateAsync(user);
            }
        }

        // 3) Добавить в роль Admin (если ещё не в ней)
        if (!await _users.IsInRoleAsync(user, "Admin"))
        {
            var addRoleRes = await _users.AddToRoleAsync(user, "Admin");
            if (!addRoleRes.Succeeded) return BadRequest(addRoleRes.Errors);
        }

        return Ok(new
        {
            message = "Admin ensured",
            user = new { user.Id, user.Email, user.UserName },
            role = "Admin"
        });
    }
}