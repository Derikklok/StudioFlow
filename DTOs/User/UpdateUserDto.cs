using System.ComponentModel.DataAnnotations;
using StudioFlow.Enums;

namespace StudioFlow.DTOs.User;

public class UpdateUserDto
{
    [Required] public string Name { get; set; } = "";
    [Required] public UserRole Role { get; set; }
    public bool IsActive { get; set; }
}