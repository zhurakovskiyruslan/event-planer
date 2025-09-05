using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Data.Entities;
using EventPlanner.Data.Enums;

namespace EventPlanner.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IUserRepository _userRepo;
    private readonly ITicketRepository _ticketRepo;
    private readonly IEventRepository _eventRepo;

    public BookingService(
        IBookingRepository bookingRepo,
        IUserRepository userRepo,
        ITicketRepository ticketRepo,
        IEventRepository eventRepo)
    {
        _bookingRepo = bookingRepo;
        _userRepo = userRepo;
        _ticketRepo = ticketRepo;
        _eventRepo = eventRepo;
    }

    /// <summary>Создать бронь.</summary>
    public async Task<Booking> CreateAsync(Booking booking)
    {
        if (booking is null)
            throw new ValidationException("Booking is required");
        var userExist = await _userRepo.ExistsAsync(booking.UserId);
        if (!userExist)
            throw new NotFoundException("User not found");
        var ticketExist = await _ticketRepo.ExistsAsync(booking.TicketId);
        if (!ticketExist)
            throw new NotFoundException("Ticket not found");
        var duplicate = await _bookingRepo.GetByUserAndTicketAsync(booking.UserId, 
            booking.TicketId);
        if (duplicate is not null && duplicate.Status == BookingStatus.Active)
            throw new ConflictException($"booking for user {booking.UserId} and " +
                                        $"ticket {booking.TicketId} already exist ");
        await _bookingRepo.AddAsync(booking);
        return booking;
    }

    /// <summary>Отмена брони. (Опционально проверяем владельца)</summary>
    public async Task CancelAsync(int bookingId, int? actorUserId = null)
    {
        var booking = await _bookingRepo.GetByIdAsync(bookingId)
                      ?? throw new NotFoundException("Booking not found");
        if (booking.Status == BookingStatus.Cancelled)
            throw new ConflictException("Booking is already cancelled");
        await _bookingRepo.CancelAsync(bookingId);
    }
    
    public async Task DeleteAsync(int id)
    {
        var bookingExists = await _bookingRepo.ExistsAsync(id);
        if (!bookingExists)
            throw new NotFoundException($"Booking with id {id} not found");
        await _bookingRepo.DeleteAsync(id);
    }

    public async Task<Booking?> GetById(int id)
    {
        var booking = await _bookingRepo.GetByIdAsync(id);
        if(booking is null)
            throw new NotFoundException($"Booking with id {id} not found");
        return await _bookingRepo.GetByIdAsync(id);
    }

    public async Task<List<Booking>> GetByUserId(int id)
    {
        var userExist = await _userRepo.ExistsAsync(id);
        if(!userExist)
            throw new NotFoundException($"userId {id} not found");
        var bookings = await _bookingRepo.GetByUserIdAsync(id);
        if(bookings.Count==0)
            throw new NotFoundException($"no bookings found for userId {id}");
        return bookings;
    }

    public async Task<List<Booking>> GetActiveBooking()
    {
        var bookings = await _bookingRepo.GetActiveBookingsAsync();
        if (!bookings.Any())
            throw new NotFoundException("No active bookings found");
        return bookings;
    }
    public async Task<List<Booking>> GetByEventId(int eventId)
    {
       var eventExist = await _eventRepo.ExistsAsync(eventId);
       if (!eventExist)
           throw new NotFoundException($"eventId {eventId} not found");
       var bookings = await _bookingRepo.GetByEventIdAsync(eventId);
       if (bookings.Count == 0)
           throw new NotFoundException($"no bookings found for eventId {eventId}");
       return bookings;
    }

    public async Task<Booking?> GetByUserAndTickets(int userId, int ticketId)
    {
        var userExist = await _userRepo.ExistsAsync(userId);
        if (!userExist) 
            throw new NotFoundException($"userId {userId} not found");
        var ticketExist = await _ticketRepo.ExistsAsync(ticketId);
        if (!ticketExist)
            throw new NotFoundException($"ticketId {ticketId} not found"); 
        var booking = await _bookingRepo.GetByUserAndTicketAsync(userId,  ticketId);
        if(booking == null)
            throw new NotFoundException($"no booking found for userId {userId} and ticketId {ticketId}");
        return booking;
    }
}