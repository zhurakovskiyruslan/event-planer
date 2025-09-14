using EventPlanner.Web.Models;
using EventPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventPlanner.Web.Controllers;

public class BookingController : Controller
{
    private readonly BookingApiClient _bookingApi;
    private readonly EventApiClient _eventApi;

    public BookingController(BookingApiClient bookingApi, EventApiClient eventApi)
    {
        _bookingApi = bookingApi;
        _eventApi = eventApi;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(int? id, int? userId, int? eventId, int? ticketId, bool? active)
    {
        await LoadEventAsync();
        ViewBag.HasQuery = id.HasValue || userId.HasValue || eventId.HasValue || ticketId.HasValue || (active ?? false);

        var list = new List<BookingVm>();

        if (id.HasValue)
        {
            var one = await _bookingApi.GetByIdAsync(id.Value);
            if (one != null) list.Add(one);
        }
        else if (userId.HasValue && ticketId.HasValue)
        {
            var one = await _bookingApi.GetByUserAndTicketAsync(userId.Value, ticketId.Value);
            if (one != null) list.Add(one);
        }
        
        else if (eventId.HasValue)
        {
            list = await _bookingApi.GetByEventIdAsync(eventId.Value);
        }
        else if (active == true)
        {
            list = await _bookingApi.GetAllActiveBookingsAsync();
        }

        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> My()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
        int domainUserId = int.Parse(userIdClaim);
        var bookings = await _bookingApi.GetByUserIdAsync(domainUserId);
        return View(bookings);
       
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(int eventId)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
        int domainUserId = int.Parse(userIdClaim);
       
        var result = await _bookingApi.CreateAsync(new UpsertBookingVm(domainUserId, eventId));
        if (result == null)
        {
            ModelState.AddModelError("", "Failed to create booking");
            return View();
        }
        return RedirectToAction();
    }

    [HttpPost]
    public async Task<IActionResult> CancelAsync(int id)
    {
        await _bookingApi.CancelAsync(id);
        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _bookingApi.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
//
//
//
//      ТУТ ОТВАЛИТСЯ
    [NonAction]
    private async Task LoadEventAsync()
    {
        var events = await _eventApi.GetAllAsync();
        ViewBag.EventList = events
            .Select(e => new SelectListItem{ Value = e.Id.ToString(), Text = e.Title })
            .ToList();
    }
}