using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Application.ReadModels;
using EventPlanner.Data.Entities;
using EventPlanner.Data.Enums;
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
        return MapToDto(entity);
    }
    public async Task<List<EventDto>> GetAllAsync()
    {
        var events = await eventRepository.GetAllAsync();
        if (!events.Any()) throw new NotFoundException("No events found");
        return events.Select(MapToDto).ToList();
    }
    
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
    
    private static EventDto MapToDto(Event entity)
    {
        var sold = entity.Tickets.SelectMany(t => t.Bookings)
            .Count(b => b.Status == BookingStatus.Active);

        return new EventDto(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.StartAtUtc,
            entity.Capacity,
            entity.Location.Name,
            entity.Capacity - sold
        );
    }
}
