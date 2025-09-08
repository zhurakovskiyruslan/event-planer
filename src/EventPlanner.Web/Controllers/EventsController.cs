using EventPlanner.Web.Models;
using EventPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventPlanner.Web.Controllers;

public class EventsController : Controller
{
    private readonly EventsApiClient _api;
    private readonly LocationsApiClient _locations;
    public EventsController(EventsApiClient api,  LocationsApiClient locations)
    {
        _api = api;
        _locations = locations;
    }
    public async Task<IActionResult> Index()
    {
        var items = await _api.GetAllAsync();
        return View(items);
    }
    
    public async Task<IActionResult> Create()
    {
        var locations = await _locations.GetAllAsync();
        ViewBag.LocationList = locations
            .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Name })
            .ToList();
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateEventVm model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var locations = await _locations.GetAllAsync();
        ViewBag.LocationList = locations
            .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Name })
            .ToList();
        var startAtUtc = DateTime.SpecifyKind(model.StartAtUtc, DateTimeKind.Local)
            .ToUniversalTime();
        var newEvent = model with { StartAtUtc = startAtUtc };
        await _api.CreateAsync(newEvent);
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _api.GetAsync(id);
        if (item is null) return NotFound();
        var locations = await _locations.GetAllAsync();
        ViewBag.LocationList = locations
            .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Name })
            .ToList();
        var vm = new UpdateEventVm(item.Title, item.Description, item.StartAtUtc, item.Capacity, item.LocationId);
        ViewBag.Id = id;
        return View(vm);

       
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, UpdateEventVm model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var locations = await _locations.GetAllAsync();
        ViewBag.LocationList = locations
            .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Name })
            .ToList();
        var startAtUtc = DateTime.SpecifyKind(model.StartAtUtc, DateTimeKind.Local)
            .ToUniversalTime();
        var renewedEvent = model with { StartAtUtc = startAtUtc };
        await _api.UpdateAsync(id, renewedEvent);
        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}