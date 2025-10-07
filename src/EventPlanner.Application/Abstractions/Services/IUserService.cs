using EventPlanner.Application.ReadModels;
using EventPlanner.Data.Entities;

namespace EventPlanner.Application.Abstractions.Services;

public interface IUserService
{
    Task<User> GetById(int id);
    Task<User> GetByEmail(string email);
    Task<User> GetByAppUserId(int appUserId);
    Task<List<UserDto>> GetAllAsync();
    Task UpdateAsync(User user);
    Task<User> CreateAsync(User user);
    Task DeleteAsync(int id);
}