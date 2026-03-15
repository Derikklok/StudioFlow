using System.ComponentModel.DataAnnotations;
using StudioFlow.Enums;

namespace StudioFlow.DTOs.User;

public class CreateUserDto
{
    [Required] [MaxLength(100)] public string Name { get; set; } = "";

    [Required] [EmailAddress] public string Email { get; set; } = "";

    [Required] public string Password { get; set; } = "";

    [Required] public UserRole Role { get; set; }
}