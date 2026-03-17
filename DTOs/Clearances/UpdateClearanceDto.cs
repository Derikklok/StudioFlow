using System.ComponentModel.DataAnnotations;

namespace StudioFlow.DTOs.Clearances;

public class UpdateClearanceDto
{
    public string? RightsOwner { get; set; }

    public string? LicenseType { get; set; }

    public string? Notes { get; set; }

    public bool? IsApproved { get; set; }
}

