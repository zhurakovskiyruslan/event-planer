using EventPlanner.Data.Entities;
namespace EventPlanner.Application.Abstractions.Services
{
    public interface ILocationService
    {
        Task<Location?> GetByIdAsync(int id);
        Task<List<Location>> GetAllAsync();
        Task<Location> CreateAsync(Location location);
        Task UpdateAsync(Location location);
        Task DeleteAsync(int id);
    }
}