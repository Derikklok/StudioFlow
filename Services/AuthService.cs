using StudioFlow.DTOs.Auth;
using StudioFlow.Exceptions;
using StudioFlow.Models;
using StudioFlow.Repositories.Interfaces;
using StudioFlow.Services.Interfaces;

namespace StudioFlow.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;

    public AuthService(IAuthRepository repository)
    {
        _repository = repository;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if user already exists
        var existingUser = await _repository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new UserAlreadyExistsException(request.Email);
        }

        // Create new user
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password, // TODO: Hash password in production
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _repository.CreateAsync(user);

        return new RegisterResponse
        {
            Id = createdUser.Id,
            Name = createdUser.Name,
            Email = createdUser.Email,
            Role = createdUser.Role,
            IsActive = createdUser.IsActive,
            CreatedAt = createdUser.CreatedAt,
            Message = "User registered successfully"
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // Find user by email
        var user = await _repository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new InvalidCredentialsException("Invalid email or password");
        }

        // Check password (currently plain text comparison - TODO: Use hashing in production)
        if (user.Password != request.Password)
        {
            throw new InvalidCredentialsException("Invalid email or password");
        }

        return new LoginResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            Message = "Login successful"
        };
    }
}

