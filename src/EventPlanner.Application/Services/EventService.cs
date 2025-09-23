using System.Text.Json;
using EventPlanner.Application.Abstractions.Cache;
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
    IValidator<Event> eventValidator,
    ICacheService cacheService) : IEventService
{
    public async Task<Event> CreateAsync(Event entity)
    {
        await eventValidator.ValidateAndThrowAsync(entity);
        var location = await locationRepository.GetByIdAsync(entity.LocationId);
        if (location == null) throw new NotFoundException($"location with id {entity.LocationId} not found");
        if (location.Capacity < entity.Capacity)
            throw new ConflictException($"The capacity of this location is only {location.Capacity} ");
        await eventRepository.AddAsync(entity);
        return entity;
    }

    public async Task<EventDto> GetById(int eventId)
    {
        var key = $"event:{eventId}";
        var cached = await cacheService.GetAsync(key);
        if (cached != null) return JsonSerializer.Deserialize<EventDto>(cached);
        var entity = await eventRepository.GetByIdAsync(eventId);
        if (entity is null) throw new NotFoundException($"Event with id {eventId} not found");
        await cacheService.SetAsync(key, JsonSerializer.Serialize(MapToDto(entity)), TimeSpan.FromMinutes(10));
        return MapToDto(entity);
    }

    public async Task<List<EventDto>> GetAllAsync(PageInfo pageInfo)
    {
        var events = await eventRepository.GetAllAsync();
        var result = await PagedList<Event>.CreateAsync(events, pageInfo.Page, pageInfo.Size);
        return result.Items.Select(MapToDto).ToList();
    }

    public async Task UpdateAsync(Event entity)
    {
        var eventExist = await eventRepository.ExistsAsync(entity.Id);
        if (!eventExist) throw new NotFoundException($"Event with id {entity.Id} not found");
        await eventValidator.ValidateAndThrowAsync(entity);
        var location = await locationRepository.GetByIdAsync(entity.LocationId);
        if (location == null) throw new NotFoundException($"location with id {entity.LocationId} not found");
        if (location.Capacity < entity.Capacity)
            throw new ConflictException($"The capacity of this location is only {location.Capacity} ");
        await eventRepository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var eventExist = await eventRepository.ExistsAsync(id);
        if (!eventExist) throw new NotFoundException($"Event with id {id} not found");
        await eventRepository.DeleteAsync(id);
    }

    private static EventDto MapToDto(Event entity)
    {
        var sold = entity.Tickets.SelectMany(t => t.Bookings).Count(b => b.Status == BookingStatus.Active);
        return new EventDto(entity.Id, entity.Title, entity.Description, entity.StartAtUtc, entity.Capacity,
            entity.LocationId, entity.Location.Name, entity.Capacity - sold);
    }
}