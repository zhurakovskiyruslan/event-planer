using EventPlanner.Data.Entities;
namespace EventPlanner.Application.Abstractions.Services;

public interface IBookingService
{ 
    Task<Booking> GetById(int id);
    Task <List<Booking>> GetByUserId(int userId);
    Task<List<Booking>> GetByEventId(int eventId);
    Task <List<Booking>> GetActiveBooking();
    Task<Booking?> GetByUserAndTickets(int userId, int ticketId);
    Task<Booking> CreateAsync(Booking booking);
    Task DeleteAsync(int bookingId);
    Task CancelAsync(int bookingId, int? actorUserId = null);
}