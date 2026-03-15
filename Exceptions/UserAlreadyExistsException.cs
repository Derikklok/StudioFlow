namespace StudioFlow.Exceptions;

/// <summary>
/// Exception thrown when attempting to register with an email that already exists
/// </summary>
public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string email)
        : base($"A user with email '{email}' already exists. Please try logging in.")
    {
        Email = email;
    }

    public string Email { get; }
}
