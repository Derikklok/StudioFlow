using System.ComponentModel.DataAnnotations;

namespace StudioFlow.DTOs.Clearances;

public class CreateClearanceDto
{
    [Required] public int SampleId { get; set; }

    [Required] [MaxLength(200)] public string RightsOwner { get; set; } = default!;

    public string? LicenseType { get; set; }

    public string? Notes { get; set; }
}