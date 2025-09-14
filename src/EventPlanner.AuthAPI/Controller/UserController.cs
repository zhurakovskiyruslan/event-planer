using EventPlanner.AuthAPI.Contracts;
using EventPlanner.AuthAPI.Data;
using EventPlanner.AuthAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.AuthAPI.Controller;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppIdentityDbContext _db;
    private readonly JwtService _jwt;

    public UserController(UserManager<ApplicationUser> userManager, AppIdentityDbContext db, JwtService jwt)
    {
        _userManager = userManager;
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("register"), AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto req)
    {
        using var tx = await _db.Database.BeginTransactionAsync();

        var user   = new ApplicationUser { UserName = req.Email, Email = req.Email };
        var create = await _userManager.CreateAsync(user, req.Password);
        if (!create.Succeeded) return BadRequest(create.Errors);

        var domain = new UserRead
        {
            Name = req.Name,
            Email = req.Email,
            CreatedAt = DateTime.UtcNow,
            AppUserId = user.Id
        };
        _db.Set<UserRead>().Add(domain);
        await _db.SaveChangesAsync();

        // роли (если надо)
        var roles = await _userManager.GetRolesAsync(user);

        var token = _jwt.Issue(user, domain.Id, roles);
        await tx.CommitAsync();

        return Ok(new {
            token,
            expiresAtUtc = DateTime.UtcNow.AddHours(1),
            user = new { id = user.Id, email = user.Email, userName = user.UserName }
        });
    }

    [HttpPost("login"), AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, req.Password))
            return Unauthorized(new { error = "Invalid email or password" });

        // на логине быстро получаем доменный id
        var domain = await _db.Set<UserRead>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.AppUserId == user.Id);

        if (domain is null)
            return Problem(detail: $"Profile for AppUserId={user.Id} not found", statusCode: 500);

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwt.Issue(user, domain.Id, roles);

        return Ok(new {
            token,
            expiresAtUtc = DateTime.UtcNow.AddHours(1),
            user = new { id = user.Id, email = user.Email, userName = user.UserName }
        });
    }
}