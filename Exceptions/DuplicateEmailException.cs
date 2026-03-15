namespace StudioFlow.Exceptions;

/// <summary>
/// Exception thrown when attempting to create a user with an email that already exists.
/// This is a business logic exception, not a system error.
/// </summary>
public class DuplicateEmailException : Exception
{
    public DuplicateEmailException(string email)
        : base($"A user with email '{email}' already exists.")
    {
        Email = email;
    }

    public string Email { get; }
}

