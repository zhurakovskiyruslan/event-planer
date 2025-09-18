using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Application.ReadModels;
using EventPlanner.Data.Entities;
using EventPlanner.Data.Enums;
using FluentValidation;

namespace EventPlanner.Application.Services;

public class BookingService(
    IBookingRepository bookingRepo,
    IUserRepository userRepo,
    ITicketRepository ticketRepo,
    IEventRepository eventRepo,
    IValidator<Booking> bookingValidator)
    : IBookingService
{
    /// <summary>Создать бронь.</summary>
    public async Task<Booking> CreateAsync(Booking booking)
    {
        await bookingValidator.ValidateAndThrowAsync(booking);
        var userExist = await userRepo.ExistsAsync(booking.UserId);
        if (!userExist)
            throw new NotFoundException("User not found");
        var ticketExist = await ticketRepo.ExistsAsync(booking.TicketId);
        if (!ticketExist)
            throw new NotFoundException("Ticket not found");
        var duplicate = await bookingRepo.GetByUserAndTicketAsync(booking.UserId, 
            booking.TicketId);
        if (duplicate is not null && duplicate.Status == BookingStatus.Active)
            throw new ConflictException($"booking for user {booking.UserId} and " +
                                        $"ticket {booking.TicketId} already exist ");
        await bookingRepo.AddAsync(booking);
        return booking;
    }

    /// <summary>Отмена брони. (Опционально проверяем владельца)</summary>
    public async Task CancelAsync(int bookingId, int? actorUserId = null)
    {
        var booking = await bookingRepo.GetByIdAsync(bookingId)
                      ?? throw new NotFoundException("Booking not found");
        if (booking.Status == BookingStatus.Cancelled)
            throw new ConflictException("Booking is already cancelled");
        await bookingRepo.CancelAsync(bookingId);
    }
    
    public async Task DeleteAsync(int id)
    {
        var bookingExists = await bookingRepo.ExistsAsync(id);
        if (!bookingExists)
            throw new NotFoundException($"Booking with id {id} not found");
        await bookingRepo.DeleteAsync(id);
    }

    public async Task<Booking> GetById(int id)
    {
        var booking = await bookingRepo.GetByIdAsync(id);
        if(booking is null)
            throw new NotFoundException($"Booking with id {id} not found");
        return booking;
    }

    public async Task<List<BookingDto>> GetByUserId(int id)
    {
        var userExist = await userRepo.ExistsAsync(id);
        if(!userExist)
            throw new NotFoundException($"userId {id} not found");
        var bookings = await bookingRepo.GetByUserIdAsync(id);
        var result = bookings.Select(MapToDto).ToList();
        if(result.Count==0)
            throw new NotFoundException($"no bookings found for userId {id}");
        return result;
    }

    public async Task<List<BookingDto>> GetActiveBooking()
    {
        var bookings = await bookingRepo.GetActiveBookingsAsync();
        return  bookings.Select(MapToDto).ToList();
    }
    
    public async Task<List<BookingDto>> GetAllAsync()
    {
        var bookings = await bookingRepo.GetAllAsync();
        var result = bookings.Select(MapToDto).ToList();
        return result;
    }
    public async Task<List<BookingDto>> GetByEventId(int eventId)
    {
       var eventExist = await eventRepo.ExistsAsync(eventId);
       if (!eventExist)
           throw new NotFoundException($"eventId {eventId} not found");
       var bookings = await bookingRepo.GetByEventIdAsync(eventId);
       var result = bookings.Select(MapToDto).ToList();
       return result;
    }

    public async Task<Booking?> GetByUserAndTickets(int userId, int ticketId)
    {
        var userExist = await userRepo.ExistsAsync(userId);
        if (!userExist) 
            throw new NotFoundException($"userId {userId} not found");
        var ticketExist = await ticketRepo.ExistsAsync(ticketId);
        if (!ticketExist)
            throw new NotFoundException($"ticketId {ticketId} not found"); 
        var booking = await bookingRepo.GetByUserAndTicketAsync(userId,  ticketId);
        if(booking == null)
            throw new NotFoundException($"no booking found for userId {userId} and ticketId {ticketId}");
        return booking;
    }

    private static BookingDto MapToDto(Booking b)
    {
        return new BookingDto(
            b.Id,
            b.Ticket.EventId,
            b.Ticket.Event.Title,
            b.Ticket.Event.Description,
            b.Ticket.Event.StartAtUtc,
            b.Ticket.Event.Location.Name,
            b.Ticket.Event.Location.Address,
            b.Ticket.Type.ToString(),
            b.Ticket.Price,
            b.Status.ToString()
        );
    }
    
}