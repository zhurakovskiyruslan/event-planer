using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Application.Common.Validation;
using EventPlanner.Data.Entities;
using Microsoft.Extensions.Options;

namespace EventPlanner.Application.Services;

public class UserService: IUserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
        {
        _userRepository = userRepository;
        }
    
    public async Task<User> GetById(int userId)
    {
        return await _userRepository.GetByIdAsync(userId)??
               throw new NotFoundException($"user with id {userId} not found");
    }
    public async Task<User> GetByEmail(string  email)
    {
        return await _userRepository.GetByEmailAsync(email)??
               throw new NotFoundException($"user with email {email} not found");;
    }
    public async Task UpdateAsync(User user)
    {
        if (user is null)
            throw new ValidationException("User is required");
        var userExist = await _userRepository.GetByIdAsync(user.Id)??
            throw new NotFoundException($"user with id {user.Id} not found");
        if(string.IsNullOrWhiteSpace(user.Name))
            throw new ValidationException("User name is required");
        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ValidationException("User email is required");
        if(!EmailValidator.IsValid(user.Email))
            throw new ValidationException("User email is invalid");
        await _userRepository.UpdateAsync(user);
    }

    public async Task<User> CreateAsync(User user)
    {
        if (user is null)
            throw new ValidationException("User is required");
        if(string.IsNullOrWhiteSpace(user.Name))
            throw new ValidationException("User name is required");
        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ValidationException("User email is required");
        if(!EmailValidator.IsValid(user.Email))
            throw new ValidationException("User email is invalid");
        var emailExist = await _userRepository.GetByEmailAsync(user.Email);
        if(emailExist != null)
            throw new ValidationException("User email already exist");
        await _userRepository.AddAsync(user);
        return user;
    }
    public async Task DeleteAsync(int userId)
    {
        var userExist = await _userRepository.GetByIdAsync(userId)??
                        throw new NotFoundException($"user with id {userId} not found");
        await _userRepository.DeleteAsync(userId);
    }    
}
