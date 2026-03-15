using Microsoft.EntityFrameworkCore;
using StudioFlow.Data;
using StudioFlow.Exceptions;
using StudioFlow.Models;
using StudioFlow.Repositories.Interfaces;

namespace StudioFlow.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
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
                throw new DuplicateEmailException(user.Email);
            }
            throw;
        }
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DisableAsync(User user)
    {
        user.IsActive = false;
        await _context.SaveChangesAsync();
    }
}