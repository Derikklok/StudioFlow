using Microsoft.EntityFrameworkCore;
using StudioFlow.Data;
using StudioFlow.Models;
using StudioFlow.Repositories.Interfaces;

namespace StudioFlow.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }

    public async Task<User> CreateAsync(User user)
    {
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        catch (DbUpdateException ex)
        {
            // Check for unique constraint violation on Email
            if (ex.InnerException?.Message.Contains("Duplicate") ?? false)
            {
                throw new InvalidOperationException($"A user with email '{user.Email}' already exists.");
            }
            throw;
        }
    }
}

