using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
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
        await _locationRepo.AddAsync(location);
        return location;
    }
    /// <summary>Получить локацию по Id</summary>
    public Task<Location?> GetByIdAsync(int id) =>
        _locationRepo.GetByIdAsync(id);

    /// <summary>Список всех локаций</summary>
    public Task<List<Location>> GetAllAsync() =>
        _locationRepo.GetAllAsync();

    /// <summary>
    /// Обновить локацию.
    /// Здесь предполагается, что доменная сущность уже изменена (через методы самой сущности),
    /// а сервис только сохраняет изменения.
    /// </summary>
    public async Task UpdateAsync(Location location)
    {
        if (location is null) throw new ArgumentNullException(nameof(location));
        await _locationRepo.UpdateAsync(location);
    }

    /// <summary>Удалить локацию по Id</summary>
    public Task DeleteAsync(int id) =>
        _locationRepo.DeleteAsync(id);
}