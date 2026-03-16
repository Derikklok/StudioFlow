using StudioFlow.Enums;

namespace StudioFlow.Models;

public class Sample
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string Title { get; set; } = default!;

    public string? SourceArtist { get; set; }

    public string? SourceTrack { get; set; }

    public string? RightsHolder { get; set; }

    public SampleStatus Status { get; set; } = SampleStatus.DRAFT;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public Project Project { get; set; } = default!;
}