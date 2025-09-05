using EventPlanner.API.Contracts;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // Создать бронь
    [HttpPost]
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
    [HttpDelete("cancel{id}")]
    public async Task<IActionResult> Cancel(int id, int? actorUserId = null)
    {
        
        await _bookingService.CancelAsync(id, actorUserId);
        return NoContent();
    }
    
    [HttpDelete("deleteBooking{id}")]
    public async Task<ActionResult> Delete(int id)
    {
       await _bookingService.DeleteAsync(id);
       return NoContent();
    }
    // Получить бронь по айди
    [HttpGet("{id}")]

    public async Task<ActionResult<BookingResponseDto>> GetById(int id)
    {
        var bookings = await _bookingService.GetById(id);
        BookingResponseDto dto = new BookingResponseDto(bookings.Id, bookings.UserId, bookings.User.Name,
            bookings.TicketId,  bookings.Status.ToString());
        return Ok(dto);
    }

    //получить все брони по заданому юзер айди
    [HttpGet("byUser/{id}")]
    public async Task<ActionResult<BookingResponseDto>> GetByUserId(int id)
    {
        var bookings = await _bookingService.GetByUserId(id);
        return Ok(bookings.Select(booking => new BookingResponseDto(booking.Id, 
            booking.UserId, booking.User.Name, booking.TicketId, booking.Status.ToString())));
    }
    
    [HttpGet("byIvent/{id}")]
    public async Task<ActionResult<BookingResponseDto>> GetByEventId(int id)
    {
        var bookings = await _bookingService.GetByEventId(id);
        return Ok(bookings.Select(booking => new BookingResponseDto(booking.Id, 
            booking.UserId, booking.User.Name, booking.TicketId, booking.Status.ToString())));
    }
    
    [HttpGet("allActiveBookings")]
    public async Task<ActionResult<BookingResponseDto>> GetActiveBooking()
    {
        var bookings = await _bookingService.GetActiveBooking();
        return Ok(bookings.Select(booking => new BookingResponseDto(booking.Id, 
            booking.UserId, booking.User.Name, booking.TicketId, booking.Status.ToString())));
    }
    
    [HttpGet("byUserAndTickets/{userId}/{ticketId}")]
    public async Task<ActionResult<BookingResponseDto>> GetByUserAndTickets(int userId, int ticketId)
    {
        var bookings = await _bookingService.GetByUserAndTickets(userId, ticketId);
        return Ok(new BookingResponseDto(bookings.Id, bookings.UserId, bookings.User.Name,
            bookings.TicketId,  bookings.Status.ToString()));
    }
    
}