using StudioFlow.Models;

namespace StudioFlow.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<Project> CreateAsync(Project project);
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<Project> UpdateAsync(Project project);
    Task DeleteAsync(Project project);
}