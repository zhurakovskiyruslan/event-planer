using System.Security.Claims;
using EventPlanner.Web.Models;
using EventPlanner.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Web.Controllers;

public class UserController : Controller
{
    private readonly UserApiClient _userApi;

    public UserController(UserApiClient userApi)
    {
        _userApi = userApi;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Index([FromQuery] string? q)
    {
        IReadOnlyList<UserVm> users;

        if (string.IsNullOrWhiteSpace(q))
        {
            users = await _userApi.GetAll();
        }
        else
        {
            var user = await _userApi.GetByEmailAsync(q);
            users = user is null ? new List<UserVm>() : new List<UserVm> { user };
        }

        return View(users);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var appUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (appUserIdClaim == null) return View();
        var domainUser = await _userApi.GetByAppUserIdAsync(int.Parse(appUserIdClaim));
        if (domainUser == null) return NotFound();
        var user = await _userApi.GetByIdAsync(domainUser.Id);
        if (user is null) return NotFound();
        return View(user);
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public Task<IActionResult> Create()
    {
        return Task.FromResult<IActionResult>(View());
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(UpsertUserVm model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _userApi.CreateAsync(model);
        if (result == null)
        {
            ModelState.AddModelError("", "Failed to create User");
            return View();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _userApi.GetByIdAsync(id);
        if (item is null) return NotFound();
        var vm = new UpsertUserVm(item.Name, item.Email);
        ViewBag.Id = id;
        return View(vm);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Edit(int id, UpsertUserVm model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _userApi.UpdateAsync(id, model);
        if (result == null)
        {
            ModelState.AddModelError("", "Failed to update User");
            return View();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        await _userApi.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}