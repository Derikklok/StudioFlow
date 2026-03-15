namespace StudioFlow.Exceptions;

/// <summary>
/// Exception thrown when user provides invalid credentials (email not found or password mismatch)
/// </summary>
public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException(string message = "Invalid email or password")
        : base(message)
    {
    }
}
