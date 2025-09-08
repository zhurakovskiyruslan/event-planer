using EventPlanner.Web.Models;
using EventPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventPlanner.Web.Controllers;

public class TicketController : Controller
{
    private readonly TicketApiClient _ticketApi;
    private readonly EventsApiClient _eventsApi;

    public TicketController(TicketApiClient ticketApi, EventsApiClient eventsApi)
    {
        _ticketApi = ticketApi;
        _eventsApi = eventsApi;
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
        var events = await _eventsApi.GetAllAsync();
        ViewBag.Events = events
            .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Title })
            .ToList();
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateTicketVm model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var events = await _eventsApi.GetAllAsync();
        ViewBag.Events = events
            .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Title })
            .ToList();
        await _ticketApi.CreateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var ticket = await _ticketApi.GetByIdAsync(id);
        if (ticket is  null) return NotFound();
        var events = await _eventsApi.GetAllAsync();
        ViewBag.Events = events
            .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Title })
            .ToList();
        var updatedTicket = new UpdateTicketVm
        {
            Type = ticket.Type,
            Price = ticket.Price,
            EventId = ticket.EventId,
        };
        ViewBag.Id = id;
        return View(updatedTicket);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, UpdateTicketVm model)
    {
        if  (!ModelState.IsValid)
            return View(model);
        var events = await _eventsApi.GetAllAsync();
        ViewBag.Events = events
            .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Title })
            .ToList();
        var ticketExist = await _ticketApi.GetByIdAsync(id); 
        if(ticketExist is null) return NotFound();
        await _ticketApi.UpdateAsync(id, model);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _ticketApi.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}