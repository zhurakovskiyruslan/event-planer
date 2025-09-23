using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Data.Entities;
using FluentValidation;

namespace EventPlanner.Application.Services;

public class LocationService(ILocationRepository locationRepo, IValidator<Location> locationValidator)
    : ILocationService
{
    public async Task<Location> CreateAsync(Location location)
    {
        await locationValidator.ValidateAndThrowAsync(location);
        await locationRepo.AddAsync(location);
        return location;
    }

    public async Task<Location?> GetByIdAsync(int id)
    {
        return await locationRepo.GetByIdAsync(id) ??
               throw new NotFoundException($"location with id {id} not found");
    }


    public async Task<List<Location>> GetAllAsync()
    {
        return await locationRepo.GetAllAsync() ??
               throw new NotFoundException($"locations not found");
    }

    public async Task UpdateAsync(Location location)
    {
        var locationExist = await locationRepo.GetByIdAsync(location.Id) ??
                            throw new NotFoundException($"location with id {location.Id} not found");
        await locationValidator.ValidateAndThrowAsync(location);
        await locationRepo.UpdateAsync(location);
    }


    public async Task DeleteAsync(int id)
    {
        var locationExist = await locationRepo.GetByIdAsync(id) ??
                            throw new NotFoundException($"location with id {id} not found");
        await locationRepo.DeleteAsync(id);
    }
}