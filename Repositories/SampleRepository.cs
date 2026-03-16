using Microsoft.EntityFrameworkCore;
using StudioFlow.Data;
using StudioFlow.Models;
using StudioFlow.Repositories.Interfaces;

namespace StudioFlow.Repositories;

public class SampleRepository : ISampleRepository
{
    private readonly AppDbContext _context;

    public SampleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Sample> CreateAsync(Sample sample)
    {
        _context.Samples.Add(sample);
        await _context.SaveChangesAsync();
        return sample;
    }

    public async Task<List<Sample>> GetByProjectIdAsync(int projectId)
    {
        return await _context.Samples
            .Where(x => x.ProjectId == projectId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Sample?> GetByIdAsync(int id)
    {
        return await _context.Samples
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Sample> UpdateAsync(Sample sample)
    {
        _context.Samples.Update(sample);
        await _context.SaveChangesAsync();
        return sample;
    }

    public async Task DeleteAsync(Sample sample)
    {
        _context.Samples.Remove(sample);
        await _context.SaveChangesAsync();
    }
}