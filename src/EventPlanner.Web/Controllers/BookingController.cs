using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EventPlanner.Web.Models;
using EventPlanner.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventPlanner.Web.Controllers;

public class BookingController : Controller
{
    private readonly BookingApiClient _bookingApi;
    private readonly EventApiClient _eventApi;
    private readonly UserApiClient _userApi;

    public BookingController(BookingApiClient bookingApi, EventApiClient eventApi,  UserApiClient userApi)
    {
        _bookingApi = bookingApi;
        _eventApi = eventApi;
        _userApi = userApi;
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
        var appUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (appUserIdClaim == null) return View();
        
        var user = await _userApi.GetByAppUserIdAsync(int.Parse(appUserIdClaim));
        if  (user is  null) return NotFound();
        var bookings = await _bookingApi.GetByUserIdAsync(user.Id);
        return View(bookings);
        
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(int ticketId)
    {
        var appUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (appUserIdClaim == null) return View();
        var domainUser = await _userApi.GetByAppUserIdAsync(int.Parse(appUserIdClaim));
        if (domainUser == null) return NotFound();
       
        var result = await _bookingApi.CreateAsync(new UpsertBookingVm(domainUser.Id, ticketId));
        if (result == null)
        {
            ModelState.AddModelError("", "Failed to create booking");
            return View();
        }

        return RedirectToAction(nameof(My));
    }

    [HttpPost]
    public async Task<IActionResult> CancelAsync(int id)
    {
        await _bookingApi.CancelAsync(id);
        return RedirectToAction(nameof(My));
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