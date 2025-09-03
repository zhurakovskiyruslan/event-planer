using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Data.Entities;

namespace EventPlanner.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;

    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task <Event> CreateAsync(Event entity)
    {
        await _eventRepository.AddAsync(entity);
        return entity;
    }

    public Task<Event> GetById(int eventId) =>
        _eventRepository.GetByIdAsync(eventId);
    public Task<List<Event>> GetAllAsync() =>
        _eventRepository.GetAllAsync();
    
    public async Task UpdateAsync(Event entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
      await _eventRepository.UpdateAsync(entity);
    }
    public async Task DeleteAsync(int id) =>
        await _eventRepository.DeleteAsync(id);
}
