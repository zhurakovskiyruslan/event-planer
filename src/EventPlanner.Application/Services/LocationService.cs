using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Data.Entities;

namespace EventPlanner.Application.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepo;

    public LocationService(ILocationRepository locationRepo)
    {
        _locationRepo = locationRepo;
    }
    /// <summary>
    /// Создать новую локацию
    /// </summary>
    public async Task<Location> CreateAsync(Location location)
    {
       
        if(location is null)
            throw new ValidationException("Location is required");
        if(string.IsNullOrWhiteSpace(location.Name))
            throw new ValidationException("Location name is required");
        if(string.IsNullOrWhiteSpace(location.Address))
            throw new ValidationException("Location address is required");
        if (location.Capacity <= 0)
            throw new ValidationException("Location capacity must be greater than 0");
        await _locationRepo.AddAsync(location);
        return location;
    }
    /// <summary>Получить локацию по Id</summary>
    public async Task<Location?> GetByIdAsync(int id)
    {
       return await _locationRepo.GetByIdAsync(id)??
              throw new NotFoundException($"location with id {id} not found");
    }

    /// <summary>Список всех локаций</summary>
    public async Task<List<Location>> GetAllAsync()
    {
        return await _locationRepo.GetAllAsync()??
               throw new NotFoundException($"locations not found");
    }
    /// <summary>
    /// Обновить локацию.
    /// Здесь предполагается, что доменная сущность уже изменена (через методы самой сущности),
    /// а сервис только сохраняет изменения.
    /// </summary>
    public async Task UpdateAsync(Location location)
    {
        var locationExists = await _locationRepo.GetByIdAsync(location.Id)??
                             throw new NotFoundException($"location with id {location.Id} not found");
       if (location is null)
            throw new ValidationException("Location is required");
       if (string.IsNullOrWhiteSpace(location.Name))
           throw new ValidationException("Location name is required");
       if (string.IsNullOrWhiteSpace(location.Address))
           throw new ValidationException("Location address is required");
       if (location.Capacity <= 0)
           throw new ValidationException("Location capacity must be greater than 0");
       await _locationRepo.UpdateAsync(location);
    }

    /// <summary>Удалить локацию по Id</summary>
    public Task DeleteAsync(int id) =>
        _locationRepo.DeleteAsync(id);
}