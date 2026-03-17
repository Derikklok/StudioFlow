namespace StudioFlow.Exceptions;

public class ClearanceNotFoundException : Exception
{
    public ClearanceNotFoundException(int id) 
        : base($"Clearance with ID {id} not found.")
    {
    }
}

