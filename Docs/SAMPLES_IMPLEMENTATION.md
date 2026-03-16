# Samples Feature Implementation

## Overview

The Samples feature is a comprehensive CRUD (Create, Read, Update, Delete) system for managing audio samples within music production projects in the StudioFlow application. Each sample belongs to a project and tracks its clearance status throughout the production workflow.

## Architecture

The Samples feature follows a layered architecture pattern:

```
Controller → Service → Repository → Database
   ↓          ↓           ↓            ↓
 Routes    Business     Data Access   Models
          Logic
```

## Components

### 1. Models (`Models/Sample.cs`)

**Sample Model** - Represents an audio sample used in a project:

```csharp
public class Sample
{
    public int Id { get; set; }                    // Primary Key
    public int ProjectId { get; set; }             // Foreign Key to Project
    public string Title { get; set; }              // Sample title (Required)
    public string? SourceArtist { get; set; }      // Original artist
    public string? SourceTrack { get; set; }       // Original track name
    public string? RightsHolder { get; set; }      // Rights holder information
    public SampleStatus Status { get; set; }       // Clearance/approval status
    public DateTime CreatedAt { get; set; }        // Creation timestamp
    public DateTime? UpdatedAt { get; set; }       // Last modification timestamp
    public Project Project { get; set; }           // Navigation property
}
```

### 2. Enums (`Enums/SampleStatus.cs`)

**SampleStatus** - Tracks the approval/clearance lifecycle of a sample:

- `DRAFT` - Initial sample state (Default)
- `PENDING_CLEARENCE` - Awaiting clearance/approval
- `APPROVED` - Sample cleared and approved
- `REJECTED` - Sample rejected and cannot be used

### 3. DTOs (Data Transfer Objects)

#### CreateSampleRequest (`DTOs/Samples/CreateSampleRequest.cs`)
Used to create a new sample with validation:
```csharp
public class CreateSampleRequest
{
    [Required] [MaxLength(255)] public string Title { get; set; }
    [MaxLength(255)] public string? SourceArtist { get; set; }
    [MaxLength(255)] public string? SourceTrack { get; set; }
    [MaxLength(255)] public string? RightsHolder { get; set; }
}
```

#### UpdateSampleRequest (`DTOs/Samples/UpdateSampleRequest.cs`)
Used to update an existing sample with all fields:
```csharp
public class UpdateSampleRequest
{
    [Required] [MaxLength(255)] public string Title { get; set; }
    [MaxLength(255)] public string? SourceArtist { get; set; }
    [MaxLength(255)] public string? SourceTrack { get; set; }
    [MaxLength(255)] public string? RightsHolder { get; set; }
    public SampleStatus Status { get; set; }
}
```

#### PatchSampleRequest (`DTOs/Samples/PatchSampleRequest.cs`)
Used for partial updates - **all fields are optional**:
```csharp
public class PatchSampleRequest
{
    [MaxLength(255)] public string? Title { get; set; }
    [MaxLength(255)] public string? SourceArtist { get; set; }
    [MaxLength(255)] public string? SourceTrack { get; set; }
    [MaxLength(255)] public string? RightsHolder { get; set; }
    public SampleStatus? Status { get; set; }
}
```

#### SampleResponse (`DTOs/Samples/SampleResponse.cs`)
Response DTO containing all sample data:
```csharp
public class SampleResponse
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; }
    public string? SourceArtist { get; set; }
    public string? SourceTrack { get; set; }
    public string? RightsHolder { get; set; }
    public SampleStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

### 4. Repository Layer

#### ISampleRepository (`Repositories/Interfaces/ISampleRepository.cs`)
Interface defining data access operations:
```csharp
public interface ISampleRepository
{
    Task<Sample> CreateAsync(Sample sample);
    Task<List<Sample>> GetByProjectIdAsync(int projectId);
    Task<Sample?> GetByIdAsync(int id);
    Task<Sample> UpdateAsync(Sample sample);
    Task DeleteAsync(Sample sample);
}
```

#### SampleRepository (`Repositories/SampleRepository.cs`)
Concrete implementation of ISampleRepository with Entity Framework Core:
- **CreateAsync**: Adds and saves a new sample
- **GetByProjectIdAsync**: Retrieves all samples for a specific project ordered by creation date (newest first)
- **GetByIdAsync**: Finds a sample by ID
- **UpdateAsync**: Updates an existing sample
- **DeleteAsync**: Removes a sample from database

### 5. Service Layer

#### ISampleService (`Services/Interfaces/ISampleService.cs`)
Interface defining business logic operations:
```csharp
public interface ISampleService
{
    Task<SampleResponse> CreateAsync(int projectId, CreateSampleRequest request);
    Task<List<SampleResponse>> GetByProjectAsync(int projectId);
    Task<SampleResponse> GetByIdAsync(int id);
    Task<SampleResponse> UpdateAsync(int id, UpdateSampleRequest request);
    Task<SampleResponse> PatchAsync(int id, PatchSampleRequest request);
    Task DeleteAsync(int id);
}
```

#### SampleService (`Services/SampleService.cs`)
Implements business logic with:
- Project existence validation before creating samples
- Exception handling (throws `ProjectNotFoundException` and `SampleNotFoundException`)
- Automatic timestamp management (UpdatedAt set on updates)
- Status tracking for sample approvals
- Consistent data transformation through the `Map()` method

### 6. Controllers

#### SamplesController (`Controllers/SamplesController.cs`)
REST API endpoints for sample management:

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/projects/{projectId}/samples` | Create new sample in project |
| GET | `/api/projects/{projectId}/samples` | Get all samples for a project |
| GET | `/api/samples/{id}` | Get sample by ID |
| PUT | `/api/samples/{id}` | Update entire sample |
| PATCH | `/api/samples/{id}` | Partial update |
| DELETE | `/api/samples/{id}` | Delete sample |

### 7. Exceptions

#### SampleNotFoundException (`Exceptions/SampleNotFoundException.cs`)
Custom exception thrown when a sample is not found:
```csharp
public class SampleNotFoundException : Exception
{
    public SampleNotFoundException(int id)
        : base($"Sample with id '{id}' not found.")
    {
    }
}
```

## Configuration

### Program.cs Setup
The Samples feature is configured in `Program.cs`:

```csharp
// Dependency injection
builder.Services.AddScoped<ISampleRepository, SampleRepository>();
builder.Services.AddScoped<ISampleService, SampleService>();

// Database context includes Samples DbSet
public class AppDbContext : DbContext
{
    public DbSet<Sample> Samples { get; set; }
}
```

### Global Exception Handler
The exception handler in `Program.cs` catches `SampleNotFoundException` and returns:
- **Status Code**: 404
- **Response**: `{ "error": "Sample with id 'X' not found." }`

## Database Schema

### Samples Table
```sql
CREATE TABLE `Samples` (
    `Id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `ProjectId` INT NOT NULL,
    `Title` LONGTEXT NOT NULL,
    `SourceArtist` LONGTEXT,
    `SourceTrack` LONGTEXT,
    `RightsHolder` LONGTEXT,
    `Status` INT NOT NULL DEFAULT 0,
    `CreatedAt` DATETIME NOT NULL,
    `UpdatedAt` DATETIME,
    CONSTRAINT `FK_Samples_Projects_ProjectId` FOREIGN KEY (`ProjectId`)
        REFERENCES `Projects`(`Id`) ON DELETE CASCADE
)
```

## Relationships

### Sample ↔ Project
- **Type**: Many-to-One (Each sample belongs to one project)
- **Foreign Key**: `Sample.ProjectId` → `Project.Id`
- **Delete Behavior**: Cascade (Deleting a project deletes all its samples)
- **Configuration**: Defined in `AppDbContext.OnModelCreating()`

## Migrations

- **Migration**: `20260316134817_AddSamples.cs` - Creates the Samples table and relationships

## API Examples

### 1. Create Sample
**Request:**
```http
POST /api/projects/1/samples
Content-Type: application/json

{
    "title": "Smooth Jazz Intro",
    "sourceArtist": "Jazz Masters",
    "sourceTrack": "Blue Horizon",
    "rightsHolder": "Jazz Masters LLC"
}
```

**Response (201 Created):**
```json
{
    "id": 1,
    "projectId": 1,
    "title": "Smooth Jazz Intro",
    "sourceArtist": "Jazz Masters",
    "sourceTrack": "Blue Horizon",
    "rightsHolder": "Jazz Masters LLC",
    "status": "DRAFT",
    "createdAt": "2026-03-16T10:30:00Z",
    "updatedAt": null
}
```

### 2. Get All Samples for a Project
**Request:**
```http
GET /api/projects/1/samples
```

**Response (200 OK):**
```json
[
    {
        "id": 1,
        "projectId": 1,
        "title": "Smooth Jazz Intro",
        "sourceArtist": "Jazz Masters",
        "sourceTrack": "Blue Horizon",
        "rightsHolder": "Jazz Masters LLC",
        "status": "DRAFT",
        "createdAt": "2026-03-16T10:30:00Z",
        "updatedAt": null
    },
    {
        "id": 2,
        "projectId": 1,
        "title": "Ambient Pad",
        "sourceArtist": "Electronic Dreams",
        "sourceTrack": "Atmosphere",
        "rightsHolder": "Electronic Dreams Inc",
        "status": "PENDING_CLEARENCE",
        "createdAt": "2026-03-16T10:35:00Z",
        "updatedAt": "2026-03-16T10:45:00Z"
    }
]
```

### 3. Get Sample by ID
**Request:**
```http
GET /api/samples/1
```

**Response (200 OK):**
```json
{
    "id": 1,
    "projectId": 1,
    "title": "Smooth Jazz Intro",
    "sourceArtist": "Jazz Masters",
    "sourceTrack": "Blue Horizon",
    "rightsHolder": "Jazz Masters LLC",
    "status": "DRAFT",
    "createdAt": "2026-03-16T10:30:00Z",
    "updatedAt": null
}
```

### 4. Update Sample
**Request:**
```http
PUT /api/samples/1
Content-Type: application/json

{
    "title": "Smooth Jazz Intro - Extended",
    "sourceArtist": "Jazz Masters",
    "sourceTrack": "Blue Horizon",
    "rightsHolder": "Jazz Masters LLC",
    "status": "PENDING_CLEARENCE"
}
```

**Response (200 OK):**
```json
{
    "id": 1,
    "projectId": 1,
    "title": "Smooth Jazz Intro - Extended",
    "sourceArtist": "Jazz Masters",
    "sourceTrack": "Blue Horizon",
    "rightsHolder": "Jazz Masters LLC",
    "status": "PENDING_CLEARENCE",
    "createdAt": "2026-03-16T10:30:00Z",
    "updatedAt": "2026-03-16T11:00:00Z"
}
```

### 5. Patch Sample (Partial Update)
**Request:**
```http
PATCH /api/samples/1
Content-Type: application/json

{
    "status": "APPROVED"
}
```

**Response (200 OK):**
```json
{
    "id": 1,
    "projectId": 1,
    "title": "Smooth Jazz Intro - Extended",
    "sourceArtist": "Jazz Masters",
    "sourceTrack": "Blue Horizon",
    "rightsHolder": "Jazz Masters LLC",
    "status": "APPROVED",
    "createdAt": "2026-03-16T10:30:00Z",
    "updatedAt": "2026-03-16T11:15:00Z"
}
```

**Note**: Only send the fields you want to update. Other fields are preserved automatically.

### 6. Delete Sample
**Request:**
```http
DELETE /api/samples/1
```

**Response (204 No Content):**
```
(Empty body)
```

## Error Handling

### Validation Errors
If required fields are missing or invalid:
```json
{
    "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "errors": {
        "Title": ["Title is required"]
    }
}
```

### Sample Not Found
When trying to get, update, or delete a non-existent sample:
```json
{
    "error": "Sample with id '999' not found."
}
```

### Project Not Found
When trying to create a sample for a non-existent project:
```json
{
    "error": "Project with id '999' not found."
}
```

## Best Practices

1. **Always validate input** - Use Required, MaxLength, and other validation attributes
2. **Handle timestamps properly** - CreatedAt is set on creation, UpdatedAt on modification
3. **Track sample status** - Use SampleStatus enum to track clearance/approval workflow
4. **Cascade delete** - Deleting a project automatically deletes all its samples
5. **Return appropriate HTTP status codes**:
   - 201 Created for successful creation
   - 200 OK for successful read/update
   - 204 No Content for successful deletion
   - 400 Bad Request for validation errors
   - 404 Not Found for missing resources

## Sample Status Workflow

```
DRAFT
  ↓
PENDING_CLEARENCE
  ↓
  ├→ APPROVED (Ready to use)
  └→ REJECTED (Cannot use)
```

## Future Enhancements

1. Add audio file storage/attachment to samples
2. Implement sample preview/playback
3. Add sample duration tracking
4. Implement clearance deadline tracking
5. Add sample batch operations
6. Create sample usage history/audit trail
7. Implement sample licensing information
8. Add sample quality/grade ratings
9. Create sample categorization/tagging system
10. Implement sample search and filtering

## Testing

To test the Samples feature:

1. **Create a project first**: POST to `/api/projects`
2. **Create a sample**: POST to `/api/projects/{projectId}/samples` with the project ID from step 1
3. **List samples**: GET `/api/projects/{projectId}/samples`
4. **Get specific sample**: GET `/api/samples/{id}`
5. **Update sample status**: PUT `/api/samples/{id}` with new status
6. **Delete sample**: DELETE `/api/samples/{id}`

## Summary

The Samples feature is fully implemented with:
- ✅ Complete CRUD operations
- ✅ Input validation with Data Annotations
- ✅ Proper exception handling
- ✅ Timestamp tracking (CreatedAt, UpdatedAt)
- ✅ Sample status lifecycle management
- ✅ Project-Sample relationships with cascade delete
- ✅ Clean layered architecture
- ✅ Dependency injection
- ✅ Entity Framework Core integration
- ✅ Database migrations
- ✅ Global exception handling
- ✅ Partial update support (PATCH)

