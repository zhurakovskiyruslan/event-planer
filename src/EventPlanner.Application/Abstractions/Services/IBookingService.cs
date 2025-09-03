using EventPlanner.Data.Entities;
namespace EventPlanner.Application.Abstractions.Services;

public interface IBookingService
{ 
    Task<Booking?> GetByIdAsync(int id);
    Task<Booking> CreateAsync(int userId, int ticketId);
    Task CancelAsync(int bookingId, int? actorUserId = null);
    Task<List<Booking>> GetByUserAsync(int userId);
}