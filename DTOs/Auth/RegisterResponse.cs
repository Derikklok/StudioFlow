using StudioFlow.Enums;

namespace StudioFlow.DTOs.Auth;

public class RegisterResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Message { get; set; } = "User registered successfully";
}
