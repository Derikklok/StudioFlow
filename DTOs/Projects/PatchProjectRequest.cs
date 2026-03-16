using System.ComponentModel.DataAnnotations;
using StudioFlow.Enums;

namespace StudioFlow.DTOs.Projects;

/// <summary>
/// DTO for partial project updates (PATCH requests).
/// All fields are optional - only send the fields you want to update.
/// </summary>
public class PatchProjectRequest
{
    [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
    public string? Title { get; set; }

    [MaxLength(255, ErrorMessage = "Artist name cannot exceed 255 characters")]
    public string? ArtistName { get; set; }

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }

    public DateTime? TargetReleaseDate { get; set; }

    public ProjectStatus? Status { get; set; }
}

