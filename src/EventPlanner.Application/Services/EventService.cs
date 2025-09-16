using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Application.ReadModels;
using EventPlanner.Data.Entities;
using FluentValidation;

namespace EventPlanner.Application.Services;

public class EventService(
    IEventRepository eventRepository,
    ILocationRepository locationRepository,
    IValidator<Event> eventValidator)
    : IEventService
{
    public async Task <Event> CreateAsync(Event entity)
    {
        await eventValidator.ValidateAndThrowAsync(entity);
        var location = await locationRepository.GetByIdAsync(entity.LocationId);
        if (location == null)
            throw new NotFoundException($"location with id {entity.LocationId} not found");
        if (location.Capacity < entity.Capacity)
            throw new ConflictException($"The capacity of this location is only {location.Capacity} ");
        await eventRepository.AddAsync(entity);
        return entity;
    }

    public async Task<EventDto> GetById(int eventId)
    {
       var entity = await eventRepository.GetByIdAsync(eventId);
       if (entity is null)
            throw new NotFoundException($"Event with id {eventId} not found");
       return entity;
    }
    public async Task<List<EventDto>> GetAllAsync() => 
        await eventRepository.GetAllAsync();
    
    public async Task UpdateAsync(Event entity)
    {
        var eventExist = await eventRepository.ExistsAsync(entity.Id);
        if (!eventExist)
            throw new NotFoundException($"Event with id {entity.Id} not found");
        await eventValidator.ValidateAndThrowAsync(entity);
        var location = await locationRepository.GetByIdAsync(entity.LocationId);
        if (location == null)
            throw new NotFoundException($"location with id {entity.LocationId} not found");
        if (location.Capacity < entity.Capacity)
            throw new ConflictException($"The capacity of this location is only {location.Capacity} ");
        await eventRepository.UpdateAsync(entity);
    }
    public async Task DeleteAsync(int id)
    {
        var eventExist = await eventRepository.ExistsAsync(id);
        if (!eventExist)
            throw new NotFoundException($"Event with id {id} not found");
        await eventRepository.DeleteAsync(id);
    }
}
