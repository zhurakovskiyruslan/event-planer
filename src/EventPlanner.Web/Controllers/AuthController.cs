using EventPlanner.Web.Models;
using EventPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    private readonly AuthApiClient _api;
    
    public AuthController(AuthApiClient api) => _api = api;
    
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginReqVm loginReq)
    {
        if (!ModelState.IsValid)
            return View();
        var result = await _api.LoginAsync(loginReq);
        if (result == null)
        {
            ModelState.AddModelError(string.Empty, "Incorrect login or password");
            return View();
        }
        Response.Cookies.Append("auth", result.Token, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure   = false,              // true если HTTPS
            Expires  = DateTimeOffset.UtcNow.AddHours(1)
        });

        return RedirectToAction("Index", "Home");
    }
    
    
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(RegisterReqVm registerReq)
    {
        if (!ModelState.IsValid)
            return View();
        var result = await _api.RegisterAsync(registerReq);
        if (result == null)
        {
            ModelState.AddModelError(string.Empty, "Incorrect login or password");
            return View();
        }
        Response.Cookies.Append("auth", result.Token, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure   = false,              // true если HTTPS
            Expires  = DateTimeOffset.UtcNow.AddHours(1)
        });

        return RedirectToAction("Index", "Home");
    }
    
    
}