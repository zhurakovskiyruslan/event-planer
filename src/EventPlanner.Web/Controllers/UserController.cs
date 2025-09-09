using EventPlanner.Web.Models;
using EventPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Web.Controllers;

public class UserController : Controller
{
    private readonly UserApiClient _userApi;

    public UserController(UserApiClient userApi)
    {
        _userApi = userApi;
    }
    public async Task<IActionResult> Index(int? id, string? email)
    {
        if (!id.HasValue && string.IsNullOrWhiteSpace(email))
            return View(model: null);
        UserVm? user = null;
        if (id.HasValue)
            user = await _userApi.GetByIdAsync(id.Value);
        else if (!string.IsNullOrWhiteSpace(email))
            user = await _userApi.GetByEmailAsync(email);

        return View(user);
    }
    public async Task<IActionResult> Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(UpsertUserVm model)
    {
        if (!ModelState.IsValid) return View(model);
        await _userApi.CreateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _userApi.GetByIdAsync(id);
        if(item is null) return NotFound();
        var vm = new UpsertUserVm(item.Name,item.Email);
        ViewBag.Id = id;
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, UpsertUserVm model)
    {
        if (!ModelState.IsValid) return View(model);
        await _userApi.UpdateAsync(id, model);
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _userApi.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}