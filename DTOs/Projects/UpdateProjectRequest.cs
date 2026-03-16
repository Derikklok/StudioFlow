using System.ComponentModel.DataAnnotations;
using StudioFlow.Enums;

namespace StudioFlow.DTOs.Projects;

public class UpdateProjectRequest
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
    public string Title { get; set; } = default!;

    [Required(ErrorMessage = "Artist name is required")]
    [MaxLength(255, ErrorMessage = "Artist name cannot exceed 255 characters")]
    public string ArtistName { get; set; } = default!;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }

    public DateTime? TargetReleaseDate { get; set; }

    public ProjectStatus? Status { get; set; }
}