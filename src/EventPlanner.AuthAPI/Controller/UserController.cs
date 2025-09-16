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
    private readonly IHttpClientFactory _httpClientFactory;

    public UserController(UserManager<ApplicationUser> userManager, 
        AppIdentityDbContext db, JwtService jwt,  IHttpClientFactory httpClientFactory)
    {
        _userManager = userManager;
        _db = db;
        _jwt = jwt;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("register"), AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto req)
    {
        using var tx = await _db.Database.BeginTransactionAsync();

        var user   = new ApplicationUser { UserName = req.Email, Email = req.Email };
        var create = await _userManager.CreateAsync(user, req.Password);
        if (!create.Succeeded) return BadRequest(create.Errors);

       //создание доменного юзера

        var client = _httpClientFactory.CreateClient("DomainApi");
        var payload = new
        {
            Name = req.Name,
            Email = req.Email,
            AppUserId = user.Id,
        };
        
        var response = await client.PostAsJsonAsync("api/user", payload);
        if (!response.IsSuccessStatusCode)
        {
            await _userManager.DeleteAsync(user);
            return StatusCode((int)response.StatusCode, "Failed to create domain user");
        }
       
        await _db.SaveChangesAsync();

        // роли (если надо)
        var roles = await _userManager.GetRolesAsync(user);

        var token = _jwt.Issue(user, roles);
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
        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwt.Issue(user, roles);

        return Ok(new
        {
            token,
            expiresAtUtc = DateTime.UtcNow.AddHours(1),
            user = new { id = user.Id, email = user.Email, userName = user.UserName }
        });
    }

    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto req)
    {
        var user = await _userManager.FindByIdAsync(req.Id.ToString()); 
        if(req.NewPassword != req.ConfirmNewPassword) return BadRequest("Passwords don't match");
        var result = await _userManager.ChangePasswordAsync(user, req.OldPassword, req.NewPassword);
        if (!result.Succeeded) return BadRequest(result.Errors);
        var token = _jwt.Issue(user);
        return Ok(new
        {
            token,
            expiresAtUtc = DateTime.UtcNow.AddHours(1),
            user = new { id = user.Id, email = user.Email, userName = user.UserName }
        });
    }
        
}
