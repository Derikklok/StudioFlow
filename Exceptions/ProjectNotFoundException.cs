namespace StudioFlow.Exceptions;

public class ProjectNotFoundException : Exception
{
    public ProjectNotFoundException(int id)
        : base($"Project with id '{id}' not found.")
    {
    }
}