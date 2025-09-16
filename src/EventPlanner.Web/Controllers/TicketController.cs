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
    public async Task<IActionResult> Index([FromQuery] string? q)
    {
        if (q != null)
        {
            int.TryParse(q, out int eventId);
            return View(await _ticketApi.GetByEventIdAsync(eventId));
        }
        return View(await _ticketApi.GetAllAsync());
    }
    
    public async Task<IActionResult> Create()
    {
        await LoadEventsAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(UpsertTicketVm model)
    {
        var result = await _ticketApi.CreateAsync(model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unknown error");
            await LoadEventsAsync();
            return View(model);
        }

        if (!ModelState.IsValid)
        {
            await LoadEventsAsync();
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Book(int eventId)
    {
        var result = await _ticketApi.GetByEventIdAsync(eventId);
        return View(result);
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
        var result = await _ticketApi.UpdateAsync(id, model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unknown error");
        }
        if (!ModelState.IsValid)
        {
            await LoadEventsAsync();
            ViewBag.Id = id;
            return View(model);
        }
        
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