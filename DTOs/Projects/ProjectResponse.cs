using StudioFlow.Enums;

namespace StudioFlow.DTOs.Projects;

public class ProjectResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = default!;

    public string ArtistName { get; set; } = default!;

    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }

    public DateTime? TargetReleaseDate { get; set; }

    public ProjectStatus Status { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}