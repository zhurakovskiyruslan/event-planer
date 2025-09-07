using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult Login(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            ModelState.AddModelError("username", "Имя обязательно");
            return View();
        }

        // Можно сохранить имя в сессию или TempData
        TempData["Username"] = username;

        // Редиректим на Home/Index
        return RedirectToAction("Index", "Home");
    }
}