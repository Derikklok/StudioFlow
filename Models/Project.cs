using System.Runtime.CompilerServices;
using StudioFlow.Enums;

namespace StudioFlow.Models;

public class Project
{
    public int Id { get; set; }

    // The = default!; is basically saying: "This field is required and you need to set it before using the object."
    public string Title { get; set; } = default!;

    public string ArtistName { get; set; } = default!;

    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }

    public DateTime? TargetReleaseDate { get; set; }

    public ProjectStatus Status { get; set; } = ProjectStatus.PRE_PRODUCTION;

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}