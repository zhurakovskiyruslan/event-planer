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
    private readonly DomainApiClient _domainApi;

    public UserController(UserManager<ApplicationUser> userManager, AppIdentityDbContext db, JwtService jwt,
        DomainApiClient domainApi)
    {
        _userManager = userManager;
        _db = db;
        _jwt = jwt;
        _domainApi = domainApi;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto req)
    {
        var user = new ApplicationUser { UserName = req.Email, Email = req.Email };
        var create = await _userManager.CreateAsync(user, req.Password);
        if (!create.Succeeded) return BadRequest();

        var response = await _domainApi.CreateUserAsync(new CreateDomainUserDto(req.Name, req.Email, user.Id));
        if (!response.IsSuccessStatusCode)
        {
            await _userManager.DeleteAsync(user);
            return BadRequest();
        }

        var jwt = _jwt.Issue(user);
        return Ok(new
        {
            jwt.Token,
            expiresAtUtc = jwt.Expires,
            user = new { id = user.Id, email = user.Email, userName = user.UserName }
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, req.Password))
            return Unauthorized(new { error = "Invalid email or password" });
        var roles = await _userManager.GetRolesAsync(user);
        var jwt = _jwt.Issue(user, roles);
        return Ok(new
        {
            jwt.Token,
            expiresAtUtc = jwt.Expires,
            user = new { id = user.Id, email = user.Email, userName = user.UserName }
        });
    }

    [HttpPost("changePassword")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto req)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        if (req.NewPassword != req.ConfirmNewPassword) return BadRequest("Passwords don't match");
        var result = await _userManager.ChangePasswordAsync(user, req.OldPassword, req.NewPassword);
        if (!result.Succeeded) return BadRequest(result.Errors);
        var jwt = _jwt.Issue(user);
        return Ok(new
        {
            jwt.Token,
            expiresAtUtc = jwt.Expires,
            user = new { id = user.Id, email = user.Email, userName = user.UserName }
        });
    }
}