using EventPlanner.Application.ReadModels;

namespace EventPlanner.Application.Abstractions.Services;
using EventPlanner.Data.Entities;

public interface IEventService
{
    Task<EventDto> GetById(int eventId);
    Task<List<EventDto>> GetAllAsync(PageInfo pageInfo);
    Task<Event> CreateAsync(Event entity);
    Task UpdateAsync(Event entity);
    Task DeleteAsync(int id);
}