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
    private readonly AppIdentityDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtService _jwtService;
    public UserController(AppIdentityDbContext context,  UserManager<ApplicationUser> userManager
    , JwtService jwtService)
    {
        _context = context;
        _userManager = userManager;
        _jwtService = jwtService;
    }

    //Register
    [HttpPost("register"), AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto req, [FromServices] AppIdentityDbContext db)
    {
        using var tx = await db.Database.BeginTransactionAsync();

        var user = new ApplicationUser { UserName = req.Email, Email = req.Email };
        var create = await _userManager.CreateAsync(user, req.Password);
        if (!create.Succeeded) return BadRequest(create.Errors);

        // создаём доменную запись строго 1:1 
        db.Set<UserRead>().Add(new UserRead
        {
            Name = req.Name,
            Email = req.Email,
            CreatedAt = DateTime.UtcNow,
            AppUserId = user.Id
        });
        await db.SaveChangesAsync();
        await tx.CommitAsync();

        var token = _jwtService.Issue(user);
        Response.Cookies.Append("Auth", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // если http
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });
        return Ok(); 
    }
    
    [HttpPost("login"), AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, req.Password))
            return Unauthorized();

        var token = _jwtService.Issue(user);
        Response.Cookies.Append("Auth", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, 
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });
        return Ok();
    }
    
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me() => Ok();
}
