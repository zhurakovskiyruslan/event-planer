using EventPlanner.Data.Entities;

namespace EventPlanner.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsAsync(int id);
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User entity);
    Task UpdateAsync(User entity);
    Task DeleteAsync(int id);
}