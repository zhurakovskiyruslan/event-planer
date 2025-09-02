namespace EventPlanner.Application.Abstractions.Repositories;

using EventPlanner.Data.Entities;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(int id);
    Task<List<Event>> GetAllAsync();
    Task<List<Event>> GetUpcomingEventsAsync();
    Task AddAsync(Event entity);
    Task UpdateAsync(Event entity);
    Task DeleteAsync(int id);
}
