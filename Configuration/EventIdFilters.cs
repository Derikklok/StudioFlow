namespace StudioFlow.Configuration;

/// <summary>
/// Custom event ID filters for suppressing specific EF Core events
/// </summary>
public static class EventIdFilters
{
    // Event ID 10000 = DbUpdateException
    public const int DbUpdateExceptionEventId = 10000;
    
    // Event ID 20102 = Failed executing DbCommand
    public const int FailedExecutingCommandEventId = 20102;
}

