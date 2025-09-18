using EventPlanner.Application.ReadModels;

namespace EventPlanner.Application.Abstractions.Repositories;

using EventPlanner.Data.Entities;

public interface IEventRepository
{
    Task<bool> ExistsAsync(int id);
    Task<Event?> GetByIdAsync(int id);
    Task<List<Event>> GetAllAsync(int page, int pageSize);
    Task<List<Event>> GetUpcomingEventsAsync();
    Task AddAsync(Event entity);
    Task UpdateAsync(Event entity);
    Task DeleteAsync(int id);
}
