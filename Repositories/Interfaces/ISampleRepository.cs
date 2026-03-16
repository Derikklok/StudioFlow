using StudioFlow.Models;

namespace StudioFlow.Repositories.Interfaces;

public interface ISampleRepository
{
    Task<Sample> CreateAsync(Sample sample);

    Task<List<Sample>> GetByProjectIdAsync(int projectId);

    Task<Sample?> GetByIdAsync(int id);

    Task<Sample> UpdateAsync(Sample sample);

    Task DeleteAsync(Sample sample);
}