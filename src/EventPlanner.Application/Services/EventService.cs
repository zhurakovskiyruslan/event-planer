using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Application.Common.Exceptions;
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
        if (entity is null)
            throw new ValidationException("Event is required");
        if (entity.Title.Length == 0 )
            throw new ValidationException("Title is required");
        if (entity.Description is null || entity.Description.Length == 0)
            throw new ValidationException("Description is required");
        if (entity.Capacity <= 0)
            throw new ValidationException("Capacity can't be less or equal 0");
        if (entity.StartAtUtc < DateTime.UtcNow)
            throw new ValidationException("Start date must be in the future");
        var locationExist = await _locationRepository.ExistsAsync(entity.LocationId);
        if (!locationExist)
            throw new NotFoundException($"location with id {entity.LocationId} not found");
        await _eventRepository.AddAsync(entity);
        return entity;
    }

    public async Task<Event> GetById(int eventId)
    {
       var entity = await _eventRepository.GetByIdAsync(eventId);
       if (entity is null)
            throw new NotFoundException($"Event with id {eventId} not found");
       return entity;
    }
    public async Task<List<Event>> GetAllAsync() => 
        await _eventRepository.GetAllAsync();
    
    public async Task UpdateAsync(Event entity)
    {
        if (entity is null) throw new NotFoundException("Event not found");
        if (entity.Title.Length == 0 )
            throw new ValidationException("Title is required");
        if (entity.Description is null || entity.Description.Length == 0)
            throw new ValidationException("Description is required");
        if (entity.StartAtUtc < DateTime.UtcNow)
            throw new ValidationException("Start date must be in the future");
        if (entity.Capacity <= 0)
            throw new ValidationException("Capacity can't be less or equal 0");
        var locationExist = await _locationRepository.ExistsAsync(entity.LocationId);
        if (!locationExist)
            throw new NotFoundException($"location with id {entity.LocationId} not found");

        await _eventRepository.UpdateAsync(entity);
    }
    public async Task DeleteAsync(int id)
    {
        var eventExist = await _eventRepository.ExistsAsync(id);
        if (!eventExist)
            throw new NotFoundException($"Event with id {id} not found");
        await _eventRepository.DeleteAsync(id);
    }
}
