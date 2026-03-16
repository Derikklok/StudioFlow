using StudioFlow.DTOs.Projects;
using StudioFlow.Exceptions;
using StudioFlow.Models;
using StudioFlow.Repositories.Interfaces;
using StudioFlow.Services.Interfaces;

namespace StudioFlow.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request)
    {
        var project = new Project
        {
            Title = request.Title,
            ArtistName = request.ArtistName,
            Description = request.Description,
            Deadline = request.Deadline,
            TargetReleaseDate = request.TargetReleaseDate,
            CreatedBy = request.CreatedBy
        };

        var created = await _projectRepository.CreateAsync(project);

        return Map(created);
    }

    public async Task<List<ProjectResponse>> GetAllAsync()
    {
        var projects = await _projectRepository.GetAllAsync();

        return projects.Select(Map).ToList();
    }

    public async Task<ProjectResponse> GetByIdAsync(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project == null)
            throw new ProjectNotFoundException(id);

        return Map(project);
    }

    public async Task<ProjectResponse> UpdateAsync(int id, UpdateProjectRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project == null)
            throw new ProjectNotFoundException(id);

        project.Title = request.Title;
        project.ArtistName = request.ArtistName;
        project.Description = request.Description;
        project.Deadline = request.Deadline;
        project.TargetReleaseDate = request.TargetReleaseDate;
        
        if (request.Status.HasValue)
            project.Status = request.Status.Value;

        project.UpdatedAt = DateTime.UtcNow;

        var updated = await _projectRepository.UpdateAsync(project);

        return Map(updated);
    }

    public async Task<ProjectResponse> PatchAsync(int id, PatchProjectRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project == null)
            throw new ProjectNotFoundException(id);

        // Only update fields that are provided (not null)
        if (!string.IsNullOrEmpty(request.Title))
            project.Title = request.Title;

        if (!string.IsNullOrEmpty(request.ArtistName))
            project.ArtistName = request.ArtistName;

        if (request.Description != null)
            project.Description = request.Description;

        if (request.Deadline.HasValue)
            project.Deadline = request.Deadline.Value;

        if (request.TargetReleaseDate.HasValue)
            project.TargetReleaseDate = request.TargetReleaseDate.Value;

        if (request.Status.HasValue)
            project.Status = request.Status.Value;

        project.UpdatedAt = DateTime.UtcNow;

        var updated = await _projectRepository.UpdateAsync(project);

        return Map(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project == null)
            throw new ProjectNotFoundException(id);

        await _projectRepository.DeleteAsync(project);
    }

    private static ProjectResponse Map(Project project)
    {
        return new ProjectResponse
        {
            Id = project.Id,
            Title = project.Title,
            ArtistName = project.ArtistName,
            Description = project.Description,
            Deadline = project.Deadline,
            TargetReleaseDate = project.TargetReleaseDate,
            Status = project.Status,
            CreatedBy = project.CreatedBy,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }
}