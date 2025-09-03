namespace EventPlanner.Application.Abstractions.Services;
using EventPlanner.Data.Entities;

public interface IEventService
{
    Task<Event> GetById(int eventId);
    Task<List<Event>> GetAllAsync();
    Task<Event> CreateAsync(Event entity);
    Task UpdateAsync(Event entity);
    Task DeleteAsync(int id);
}