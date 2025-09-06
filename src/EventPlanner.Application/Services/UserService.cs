using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Data.Entities;

namespace EventPlanner.Application.Services;

public class UserService: IUserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
        {
        _userRepository = userRepository;
        }
    
    public Task<User> GetById(int userId) => 
        _userRepository.GetByIdAsync(userId);
    
    public Task<User> GetByEmail(string  email) => 
        _userRepository.GetByEmailAsync(email);

    public async Task UpdateAsync(User user)
    {
        if(user == null) throw new ArgumentNullException(nameof(user));
        await _userRepository.UpdateAsync(user);
    }

    public async Task<User> CreateAsync(User user)
    {
        if(user == null) throw new ArgumentNullException(nameof(user));
        await _userRepository.AddAsync(user);
        return user;
    }
    public async Task DeleteAsync(int userId)=>
        await _userRepository.DeleteAsync(userId);
    
}
