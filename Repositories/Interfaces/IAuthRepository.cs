using StudioFlow.Models;

namespace StudioFlow.Repositories.Interfaces;

public interface IAuthRepository
{
    /// <summary>
    /// Get user by email
    /// </summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Create a new user for registration
    /// </summary>
    Task<User> CreateAsync(User user);
}

