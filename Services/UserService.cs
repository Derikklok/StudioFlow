using Microsoft.EntityFrameworkCore;
using StudioFlow.DTOs.User;
using StudioFlow.Models;
using StudioFlow.Repositories.Interfaces;
using StudioFlow.Services.Interfaces;

namespace StudioFlow.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    // Create User
    public async Task<User> CreateUser(CreateUserDto dto)
    {
        try
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                Role = dto.Role
            };

            return await _repository.CreateAsync(user);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("Duplicate") ?? false)
            {
                throw new InvalidOperationException("A user with this email address already exists.", ex);
            }
            throw;
        }
    }

    // Get All Users
    public async Task<List<User>> GetAllUsers()
    {
        return await _repository.GetAllAsync();
    }

    // Get User by id
    public async Task<User?> GetUserById(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    // Update user
    public async Task<bool> UpdateUser(int id, UpdateUserDto dto)
    {
        var user = await _repository.GetByIdAsync(id);

        if (user == null)
            return false;

        user.Name = dto.Name;
        user.Role = dto.Role;
        user.IsActive = dto.IsActive;

        await _repository.UpdateAsync(user);

        return true;
    }

    // Disable user
    public async Task<bool> DisableUser(int id)
    {
        var user = await _repository.GetByIdAsync(id);

        if (user == null)
            return false;

        await _repository.DisableAsync(user);

        return true;
    }
}