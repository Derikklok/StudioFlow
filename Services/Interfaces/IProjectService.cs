using StudioFlow.DTOs.Projects;

namespace StudioFlow.Services.Interfaces;

public interface IProjectService
{
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request);

    Task<List<ProjectResponse>> GetAllAsync();

    Task<ProjectResponse> GetByIdAsync(int id);

    Task<ProjectResponse> UpdateAsync(int id, UpdateProjectRequest request);

    Task DeleteAsync(int id);
}