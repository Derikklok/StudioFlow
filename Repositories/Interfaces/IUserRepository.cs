using StudioFlow.Models;

namespace StudioFlow.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);

    Task<List<User>> GetAllAsync();

    Task<User?> GetByIdAsync(int id);

    Task UpdateAsync(User user);

    Task DisableAsync(User user);
}