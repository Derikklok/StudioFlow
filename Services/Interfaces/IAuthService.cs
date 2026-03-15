using StudioFlow.DTOs.Auth;

namespace StudioFlow.Services.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Registers a new user with the provided credentials
    /// </summary>
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
