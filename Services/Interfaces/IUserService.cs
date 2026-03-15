using StudioFlow.DTOs.User;
using StudioFlow.Models;

namespace StudioFlow.Services.Interfaces;

public interface IUserService
{
    Task<User> CreateUser(CreateUserDto dto);
    
    Task<List<User>> GetAllUsers();
    
    Task<User?> GetUserById(int id);
    
    Task<bool> UpdateUser(int id, UpdateUserDto dto);
    
    Task<bool> DisableUser(int id);
}