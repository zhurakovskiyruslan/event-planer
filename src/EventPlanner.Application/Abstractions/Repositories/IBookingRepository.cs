using EventPlanner.Data.Entities;

namespace EventPlanner.Application.Abstractions.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int id);
    Task<List<Booking>> GetByUserIdAsync(int userId);
    Task<List<Booking>> GetActiveBookingsAsync();
    Task AddAsync(Booking entity);
    Task UpdateAsync(Booking entity);
    Task CancelAsync(int id);
    Task<Booking?> GetByUserAndTicketAsync(int userId, int ticketId);
}