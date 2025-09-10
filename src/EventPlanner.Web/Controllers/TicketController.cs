using EventPlanner.Web.Models;
using EventPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventPlanner.Web.Controllers;

public class TicketController : Controller
{
    private readonly TicketApiClient _ticketApi;
    private readonly EventApiClient _eventApi;

    public TicketController(TicketApiClient ticketApi, EventApiClient eventApi)
    {
        _ticketApi = ticketApi;
        _eventApi = eventApi;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(int? id)
    {
        ViewBag.Searched = id.HasValue; // чтобы понимать, искали ли вообще
        if (!id.HasValue) return View(model: null);
        var ticket = await _ticketApi.GetByIdAsync(id.Value); // null если не найден
        return View(ticket); // модель = TicketVm? (может быть null)
    }
    
    public async Task<IActionResult> Create()
    {
        await LoadEventsAsync();
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(UpsertTicketVm model)
    {
        if (!ModelState.IsValid)
        {
            await LoadEventsAsync();
            return View(model);
        }
        await _ticketApi.CreateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var ticket = await _ticketApi.GetByIdAsync(id);
        if (ticket is  null) return NotFound();
        await LoadEventsAsync();
        var updatedTicket = new UpsertTicketVm()
        {
            Type = ticket.Type,
            Price = ticket.Price,
            EventId = ticket.EventId,
        };
        ViewBag.Id = id;
        return View(updatedTicket);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, UpsertTicketVm model)
    {
        if (!ModelState.IsValid)
        {
            await LoadEventsAsync();
            ViewBag.Id = id;
            return View(model);
        }
        await _ticketApi.UpdateAsync(id, model);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _ticketApi.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
    
    [NonAction]
    private async Task LoadEventsAsync()
    {
        var events = await _eventApi.GetAllAsync();
        ViewBag.Events = events.Select(e => new SelectListItem
        {
            Value = e.Id.ToString(),
            Text  = e.Title
        }).ToList();
    }
}