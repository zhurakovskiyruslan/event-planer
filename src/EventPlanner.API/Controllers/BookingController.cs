using EventPlanner.API.Contracts;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Application.ReadModels;
using EventPlanner.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // Создать бронь
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<BookingResponseDto>> Create([FromBody] CreateBookingDto dto )
    {
        var booking = new Booking()
        {
            UserId = dto.UserId,
            TicketId = dto.TicketId
        };
        var result = await _bookingService.CreateAsync(booking);
        var response = await _bookingService.GetById(result.Id);
        var responseDto = new BookingResponseDto(response.Id, response.UserId, response.User.Name, 
            response.TicketId, response.Status.ToString());
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, responseDto);
    }

    // Отменить бронь
    [HttpDelete("cancel/{id}")]
    [Authorize]
    public async Task<IActionResult> Cancel(int id, int? actorUserId = null)
    {
        await _bookingService.CancelAsync(id, actorUserId);
        return NoContent();
    }
    
    [HttpDelete("deleteBooking/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> Delete(int id)
    {
       await _bookingService.DeleteAsync(id);
       return NoContent();
    }
    // Получить бронь по айди
    [HttpGet("{id}")]
    [Authorize]

    public async Task<ActionResult<BookingResponseDto>> GetById(int id)
    {
        var bookings = await _bookingService.GetById(id);
        BookingResponseDto dto = new BookingResponseDto(bookings.Id, bookings.UserId, bookings.User.Name,
            bookings.TicketId,  bookings.Status.ToString());
        return Ok(dto);
    }

    //получить все брони по заданому юзер айди
    [HttpGet("byUser/{id}")]
    [Authorize]
    public async Task<ActionResult<List<BookingDto>>> GetByUserId(int id) => await _bookingService.GetByUserId(id);
    
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<List<BookingDto>>> GetAll() => await _bookingService.GetAllAsync();
    
    [HttpGet("byEvent/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<BookingDto>> GetByEventId(int id)
    {
        
        return Ok(await _bookingService.GetByEventId(id));
    }
    
    [HttpGet("allActiveBookings")]
    public async Task<ActionResult<List<BookingDto>>> GetActiveBooking() => await _bookingService.GetActiveBooking();
    
    [HttpGet("byUserAndTickets/{userId}/{ticketId}")]
    public async Task<ActionResult<BookingResponseDto>> GetByUserAndTickets(int userId, int ticketId)
    {
        var bookings = await _bookingService.GetByUserAndTickets(userId, ticketId);
        return Ok(new BookingResponseDto(bookings.Id, bookings.UserId, bookings.User.Name,
            bookings.TicketId,  bookings.Status.ToString()));
    }
    
}