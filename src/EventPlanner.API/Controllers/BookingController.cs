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
    private readonly IUserService _userService;
    private readonly IEventService _eventService;
  

    public BookingController(IBookingService bookingService, IUserService userService, IEventService eventService)
    {
        _bookingService = bookingService;
        _userService = userService;
        _eventService = eventService;
        
    }

    // Создать бронь
    [HttpPost]
    public async Task<ActionResult<BookingResponseDto>> Create([FromBody] CreateBookingDto dto )
    {
        var booking = new Booking()
        {
            UserId = dto.UserId,
            TicketId = dto.TicketId,
        };
        var result = await _bookingService.CreateAsync(booking);
        var user = await _userService.GetById(booking.UserId);
        
        
        var entity = await _eventService.GetById(booking.TicketId);
        var response = new BookingResponseDto(booking.Id,
            booking.UserId, user.Name, booking.TicketId, 
            booking.Status);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, response);
    }

    // Отменить бронь
    [HttpDelete("cancelBooking{id}")]
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
        if (bookings is null)
            return NotFound();
        BookingResponseDto dto = new BookingResponseDto(bookings.Id, bookings.UserId, bookings.User.Name,
            bookings.TicketId,  bookings.Status);
        return Ok(dto);
    }

    //получить все брони по заданому юзер айди
    [HttpGet("byUser/{id}")]
    public async Task<ActionResult<BookingResponseDto>> GetByUserId(int id)
    {
        var bookings = await _bookingService.GetByUserId(id);
        if (bookings is null)
            return NotFound();
        return Ok(bookings.Select(booking => new BookingResponseDto(booking.Id, 
            booking.UserId, booking.User.Name, booking.TicketId, booking.Status)));
    }
    
    [HttpGet("byIvent/{id}")]
    public async Task<ActionResult<BookingResponseDto>> GetByEventId(int id)
    {
        var bookings = await _bookingService.GetByEventId(id);
        if (bookings is null)
            return NotFound();
        return Ok(bookings.Select(booking => new BookingResponseDto(booking.Id, 
            booking.UserId, booking.User.Name, booking.TicketId, booking.Status)));
    }
    
    [HttpGet("allActiveBookings")]
    public async Task<ActionResult<BookingResponseDto>> GetActiveBooking()
    {
        var bookings = await _bookingService.GetActiveBooking();
        if (bookings is null)
            return NotFound();
        return Ok(bookings.Select(booking => new BookingResponseDto(booking.Id, 
            booking.UserId, booking.User.Name, booking.TicketId, booking.Status)));
    }
    
    [HttpGet("byUserAndTickets/{userId}/{ticketId}")]
    public async Task<ActionResult<BookingResponseDto>> GetByUserAndTickets(int userId, int ticketId)
    {
        var bookings = await _bookingService.GetByUserAndTickets(userId, ticketId);
        if (bookings is null)
            return NotFound();
        var user = await _userService.GetById(userId);
         return Ok(new BookingResponseDto(bookings.Id, bookings.UserId, user.Name,
            bookings.TicketId,  bookings.Status));
    }
    
}