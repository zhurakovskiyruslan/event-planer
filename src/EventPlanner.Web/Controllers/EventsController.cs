using EventPlanner.Web.Models;
using EventPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Web.Controllers;

public class EventsController : Controller
{
    private readonly EventsApiClient _api;
    public EventsController(EventsApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var items = await _api.GetAllAsync();
        return View(items);
    }
    
    public IActionResult Create() => View();
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateEventVm model)
    {
        if (!ModelState.IsValid) return View(model);
        try
        {
            await _api.CreateAsync(model);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        await _api.CreateAsync(model);
        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}