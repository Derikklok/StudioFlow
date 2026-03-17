using StudioFlow.Models;

namespace StudioFlow.Repositories.Interfaces;

public interface IClearanceRepository
{
    Task<Clearance?> GetByIdAsync(int id);

    Task<List<Clearance>> GetAllAsync();

    Task AddAsync(Clearance clearance);

    Task SaveChangesAsync();
}