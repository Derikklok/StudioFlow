using System.ComponentModel.DataAnnotations;
using StudioFlow.Enums;

namespace StudioFlow.DTOs.Samples;

public class PatchSampleRequest
{
    [MaxLength(255)] public string? Title { get; set; }

    [MaxLength(255)] public string? SourceArtist { get; set; }

    [MaxLength(255)] public string? SourceTrack { get; set; }

    [MaxLength(255)] public string? RightsHolder { get; set; }

    public SampleStatus? Status { get; set; }
}