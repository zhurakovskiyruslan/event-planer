using System.IdentityModel.Tokens.Jwt;
using EventPlanner.AuthAPI.Contracts;
using EventPlanner.AuthAPI.Data;
using EventPlanner.AuthAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        var user = new ApplicationUser { UserName = req.Email, Email = req.Email };
        var create = await _userManager.CreateAsync(user, req.Password);
        if (!create.Succeeded)
            return BadRequest(create.Errors);

        _db.Set<UserRead>().Add(new UserRead {
            Name = req.Name,
            Email = req.Email,
            CreatedAt = DateTime.UtcNow,
            AppUserId = user.Id
        });
        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        var token = await _jwt.IssueAsync(user);
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

        var token = await _jwt.IssueAsync(user);
        return Ok(new {
            token,
            expiresAtUtc = DateTime.UtcNow.AddHours(1),
            user = new { id = user.Id, email = user.Email, userName = user.UserName }
        });
    }
}