using EventPlanner.Application.ReadModels;
using EventPlanner.Data.Entities;

namespace EventPlanner.Application.Abstractions.Repositories;

public interface IBookingRepository
{
    Task<bool> ExistsAsync(int id);
    Task<Booking?> GetByIdAsync(int id);
    Task<List<Booking>> GetAllAsync();
    Task<List<Booking>> GetByUserIdAsync(int userId);
    Task<List<Booking>> GetActiveBookingsAsync();
    Task<List<Booking>> GetByEventIdAsync(int eventId);
    Task AddAsync(Booking entity);
    Task UpdateAsync(Booking entity);
    Task CancelAsync(int id);
    Task DeleteAsync(int id);
    Task<Booking?> GetByUserAndTicketAsync(int userId, int ticketId);
}