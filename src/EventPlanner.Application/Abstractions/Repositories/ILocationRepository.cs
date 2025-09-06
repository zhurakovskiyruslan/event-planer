namespace EventPlanner.Application.Abstractions.Repositories;

using EventPlanner.Data.Entities;

public interface ILocationRepository
{
    Task<bool> ExistsAsync(int id);
    Task<Location?> GetByIdAsync(int id);
    Task<List<Location>> GetAllAsync();
    Task AddAsync(Location entity);
    Task UpdateAsync(Location entity);
    Task DeleteAsync(int id);
}
