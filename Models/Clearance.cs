namespace StudioFlow.Models;

public class Clearance
{
    public int Id { get; set; }
    
    public int SampleId { get; set; }

    public string RightsOwner { get; set; } = default!;
    
    public string? LicenseType { get; set; }

    public bool IsApproved { get; set; } = false;
    
    public DateTime? ApprovedAt { get; set; }
    
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    //Navigation Property
    public Sample Sample { get; set; } = default!;

} 