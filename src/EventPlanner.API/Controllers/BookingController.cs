using EventPlanner.Application.Services;
using EventPlanner.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly BookingService _bookingService;

    public BookingController(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // Создать бронь
    [HttpPost]
    public async Task<ActionResult<Booking>> Create(int userId, int ticketId)
    {
        try
        {
            var booking = await _bookingService.CreateAsync(userId, ticketId);
            return Ok(booking);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Отменить бронь
    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(int id, int? actorUserId = null)
    {
        try
        {
            await _bookingService.CancelAsync(id, actorUserId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Получить все брони пользователя
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<Booking>>> GetByUser(int userId)
    {
        var bookings = await _bookingService.GetByUserAsync(userId);
        return Ok(bookings);
    }
}