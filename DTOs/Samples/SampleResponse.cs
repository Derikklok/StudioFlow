using StudioFlow.Enums;

namespace StudioFlow.DTOs.Samples;

public class SampleResponse
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string Title { get; set; } = default!;

    public string? SourceArtist { get; set; }

    public string? SourceTrack { get; set; }

    public string? RightsHolder { get; set; }

    public SampleStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}