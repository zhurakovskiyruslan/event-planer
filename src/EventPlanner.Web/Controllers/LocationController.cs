using EventPlanner.Web.Models;
using EventPlanner.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Web.Controllers;

public class LocationController : Controller
{
    private readonly LocationApiClient _api;
    public LocationController(LocationApiClient api) => _api = api;
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Index()
    {
        var items = await _api.GetAllAsync();
        return View(items);
    }
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Create() => View();

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(UpsertLocationVm model)
    {
        if (!ModelState.IsValid) return View(model);
        await _api.CreateAsync(model);
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _api.GetAsync(id);
        if (item is null) return NotFound();
        var vm = new UpsertLocationVm(item.Name, item.Address, item.Capacity);
        ViewBag.Id = id; // чтобы знать кого апдейтить
        return View(vm);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Edit(int id, UpsertLocationVm model)
    {
        if (!ModelState.IsValid) return View(model);
        await _api.UpdateAsync(id, model);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}