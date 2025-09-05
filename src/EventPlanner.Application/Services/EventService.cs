using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Data.Entities;

namespace EventPlanner.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly ILocationRepository _locationRepository;

    public EventService(IEventRepository eventRepository,  ILocationRepository locationRepository)
    {
        _eventRepository = eventRepository;
        _locationRepository = locationRepository;
    }

    public async Task <Event> CreateAsync(Event entity)
    {
        if (entity.LocationId == null)
            throw new Exception("LocationId is null");
        var locationExist = await _locationRepository.ExistsAsync(entity.LocationId);
        if (!locationExist)
            throw new InvalidOperationException($"location with id {entity.LocationId} not found");
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
