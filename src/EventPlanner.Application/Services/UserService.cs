using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Application.Common.Validation;
using EventPlanner.Application.ReadModels;
using EventPlanner.Data.Entities;
using FluentValidation;

namespace EventPlanner.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IValidator<User> userValidator)
    : IUserService
{
    public async Task<User> GetById(int userId)
    {
        return await userRepository.GetByIdAsync(userId) ??
               throw new NotFoundException($"user with id {userId} not found");
    }

    public async Task<User> GetByEmail(string email)
    {
        return await userRepository.GetByEmailAsync(email) ??
               throw new NotFoundException($"user with email {email} not found");
        ;
    }

    public async Task<User> GetByAppUserId(int appUserId)
    {
        return await userRepository.GetByAppUserId(appUserId) ??
               throw new NotFoundException($"user with application user id {appUserId} not found");
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await userRepository.GetAllAsync() ??
                    throw new NotFoundException("user list not found");
        var result = users.Select(u => new UserDto(
            u.Id,
            u.Name,
            u.Email)).ToList();
        return result;
    }

    public async Task UpdateAsync(User user)
    {
        await userValidator.ValidateAndThrowAsync(user);
        var userExist = await userRepository.GetByIdAsync(user.Id) ??
                        throw new NotFoundException($"user with id {user.Id} not found");
        await userRepository.UpdateAsync(user);
    }

    public async Task<User> CreateAsync(User user)
    {
        var emailExist = await userRepository.GetByEmailAsync(user.Email);
        if (emailExist != null)
            throw new ConflictException("User email already exist");
        await userValidator.ValidateAndThrowAsync(user);
        await userRepository.AddAsync(user);
        return user;
    }

    public async Task DeleteAsync(int userId)
    {
        var userExist = await userRepository.GetByIdAsync(userId) ??
                        throw new NotFoundException($"user with id {userId} not found");
        await userRepository.DeleteAsync(userId);
    }
}