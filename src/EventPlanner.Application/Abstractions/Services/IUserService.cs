using EventPlanner.Data.Entities;

namespace EventPlanner.Application.Abstractions.Services;

public interface IUserService
{
    Task<User> GetById(int id);
    Task<User> GetByEmail(string email);
    Task UpdateAsync(User user);
    Task<User> CreateAsync(User user);
    Task DeleteAsync(int id);
}