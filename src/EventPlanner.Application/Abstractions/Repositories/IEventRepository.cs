using EventPlanner.Application.ReadModels;

namespace EventPlanner.Application.Abstractions.Repositories;

using EventPlanner.Data.Entities;

public interface IEventRepository
{
    Task<bool> ExistsAsync(int id);
    Task<EventDto?> GetByIdAsync(int id);
    Task<List<EventDto>> GetAllAsync();
    Task<List<Event>> GetUpcomingEventsAsync();
    Task AddAsync(Event entity);
    Task UpdateAsync(Event entity);
    Task DeleteAsync(int id);
}
