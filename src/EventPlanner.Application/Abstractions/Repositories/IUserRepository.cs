using EventPlanner.Application.ReadModels;
using EventPlanner.Data.Entities;

namespace EventPlanner.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsAsync(int id);
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByAppUserId(int appUserId);
    Task<List<UserDto>> GetAllAsync();
    Task AddAsync(User entity);
    Task UpdateAsync(User entity);
    Task DeleteAsync(int id);
}