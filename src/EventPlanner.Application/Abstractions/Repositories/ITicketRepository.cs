using EventPlanner.Data.Entities;

namespace EventPlanner.Application.Abstractions.Repositories;

public interface ITicketRepository
{
    Task<bool>ExistsAsync(int id);
    Task<Ticket?> GetByIdAsync(int id);
    Task<List<Ticket>> GetByEventIdAsync(int eventId);
    Task AddAsync(Ticket entity);
    Task UpdateAsync(Ticket entity);
    Task DeleteAsync(int id);
}