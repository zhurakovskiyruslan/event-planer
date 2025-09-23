using EventPlanner.Application.ReadModels;
using EventPlanner.Data.Entities;
namespace EventPlanner.Application.Abstractions.Services;

public interface IBookingService
{ 
    Task<Booking> GetById(int id);
    Task<List<BookingDto>> GetAllAsync();
    Task <List<BookingDto>> GetByUserId(int userId);
    Task<List<BookingDto>> GetByEventId(int eventId);
    Task <List<BookingDto>> GetActiveBooking();
    Task<Booking?> GetByUserAndTickets(int userId, int ticketId);
    Task<Booking> CreateAsync(Booking booking);
    Task DeleteAsync(int bookingId);
    Task CancelAsync(int bookingId, int? actorUserId = null);
}