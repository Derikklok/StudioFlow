using System.ComponentModel.DataAnnotations;

namespace StudioFlow.DTOs.Samples;

public class CreateSampleRequest
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(255)]
    public string Title { get; set; } = default!;

    [MaxLength(255)] public string? SourceArtist { get; set; }

    [MaxLength(255)] public string? SourceTrack { get; set; }

    [MaxLength(255)] public string? RightsHolder { get; set; }
}