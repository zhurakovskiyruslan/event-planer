using EventPlanner.Web.Models;
using EventPlanner.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventPlanner.Web.Controllers;

public class EventController : Controller
{
    private readonly EventApiClient _api;
    private readonly LocationApiClient _location;
    public EventController(EventApiClient api,  LocationApiClient location)
    {
        _api = api;
        _location = location;
    }
    public async Task<IActionResult> Index(int page, int size)
    {
        var items = await _api.GetAllAsync(page, size);
        return View(items);
    }
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create()
    {
        await LoadLocationAsync();
        return View();
    }
    
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(UpsertEventVm model)
    {
        
        var location = await _location.GetAsync(model.LocationId);
        if (location!.Capacity < model.Capacity)
        {
            ModelState.AddModelError("Capacity", $"Capacity of this location is only {location.Capacity}");
        }
        if (!ModelState.IsValid){
            await LoadLocationAsync();
            return View(model);
        }
       
        await _api.CreateAsync(ConvertTimeToUtc(model));
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _api.GetAsync(id);
        if (item is null) return NotFound();
        await LoadLocationAsync();
        var vm = new UpsertEventVm(
            item.Title,
            item.Description,
            item.StartAtUtc.ToLocalTime(),
            item.Capacity,
            item.LocationId
        );
        ViewBag.Id = id;
        return View(vm);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Edit(int id, UpsertEventVm model)
    {
        var location = await _location.GetAsync(model.LocationId);
        if (location!.Capacity < model.Capacity)
        {
            ModelState.AddModelError("Capacity", $"Capacity of this location is only {location.Capacity}");
        }
        if (!ModelState.IsValid){
            await LoadLocationAsync();
            return View(model);
        }
        await _api.UpdateAsync(id, ConvertTimeToUtc(model));
        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var item = await _api.GetAsync(id);
        if (item is null) return NotFound();
        return View(item);
    }

    [NonAction]
    private async Task LoadLocationAsync()
    {
        var locations = await _location.GetAllAsync();
        ViewBag.LocationList = locations
            .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Name })
            .ToList();
    }

    [NonAction]
    private UpsertEventVm ConvertTimeToUtc(UpsertEventVm model)
    {
        var startAtUtc = DateTime.SpecifyKind(model.StartAtUtc, DateTimeKind.Local)
            .ToUniversalTime();
        var renewedEvent = model with { StartAtUtc = startAtUtc };
        return renewedEvent;
    }
    
}