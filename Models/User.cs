using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StudioFlow.Enums;

namespace StudioFlow.Models;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    public int Id { get; set; }

    [Required] [MaxLength(100)] public string Name { get; set; } = "";

    [Required] [EmailAddress] public string Email { get; set; } = "";

    [Required] public string Password { get; set; } = "";

    [Required] public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}