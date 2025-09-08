using EventPlanner.Web.Models;
using EventPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Web.Controllers;

public class LocationsController : Controller
{
    private readonly LocationsApiClient _api;
    public LocationsController(LocationsApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var items = await _api.GetAllAsync();
        return View(items);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(CreateLocationVm model)
    {
        if (!ModelState.IsValid) return View(model);
        await _api.CreateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _api.GetAsync(id);
        if (item is null) return NotFound();
        var vm = new UpdateLocationVm(item.Name, item.Address, item.Capacity);
        ViewBag.Id = id; // чтобы знать кого апдейтить
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, UpdateLocationVm model)
    {
        if (!ModelState.IsValid) return View(model);
        await _api.UpdateAsync(id, model);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}