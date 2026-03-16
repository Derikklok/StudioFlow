namespace StudioFlow.Exceptions;

public class SampleNotFoundException : Exception
{
    public SampleNotFoundException(int id)
        : base($"Sample with id '{id}' not found.")
    {
    }
}