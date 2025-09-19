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

    public BookingController(BookingApiClient bookingApi, EventApiClient eventApi, UserApiClient userApi)
    {
        _bookingApi = bookingApi;
        _eventApi = eventApi;
        _userApi = userApi;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Index(bool activeOnly = false)
    {
        return activeOnly ? View(await _bookingApi.GetAllActiveBookingsAsync()) : View(await _bookingApi.GetAllAsync());
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> My()
    {
        var appUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (appUserIdClaim == null) return View();
        var user = await _userApi.GetByAppUserIdAsync(int.Parse(appUserIdClaim));
        if (user is null) return NotFound();
        var bookings = await _bookingApi.GetByUserIdAsync(user.Id);
        return View(bookings);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
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
    [Authorize]
    public async Task<IActionResult> CancelAsync(int id)
    {
        await _bookingApi.CancelAsync(id);
        return RedirectToAction(nameof(My));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _bookingApi.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}