using Microsoft.EntityFrameworkCore;
using StudioFlow.Data;
using StudioFlow.Models;
using StudioFlow.Repositories.Interfaces;

namespace StudioFlow.Repositories;

public class ClearanceRepository : IClearanceRepository
{
    private readonly AppDbContext _context;

    public ClearanceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Clearance?> GetByIdAsync(int id)
    {
        return await _context.Clearances
            .Include(c => c.Sample)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Clearance>> GetAllAsync()
    {
        return await _context.Clearances
            .Include(c => c.Sample)
            .ToListAsync();
    }

    public async Task AddAsync(Clearance clearance)
    {
        await _context.Clearances.AddAsync(clearance);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}