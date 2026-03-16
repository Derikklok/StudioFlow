# Projects Feature Implementation

## Overview

The Projects feature is a comprehensive CRUD (Create, Read, Update, Delete) system that allows users to manage music production projects in the StudioFlow application. Each project can be tracked through various stages of production from pre-production to release.

## Architecture

The Projects feature follows a layered architecture pattern:

```
Controller → Service → Repository → Database
   ↓          ↓           ↓            ↓
 Routes    Business     Data Access   Models
          Logic
```

## Components

### 1. Models (`Models/Project.cs`)

**Project Model** - Represents a music production project with the following fields:

```csharp
public class Project
{
    public int Id { get; set; }                    // Primary Key
    public string Title { get; set; }              // Project title (Required)
    public string ArtistName { get; set; }         // Artist name (Required)
    public string? Description { get; set; }       // Optional description
    public DateTime? Deadline { get; set; }        // Optional project deadline
    public DateTime? TargetReleaseDate { get; set; } // Optional target release
    public ProjectStatus Status { get; set; }      // Current project status
    public int CreatedBy { get; set; }             // User ID who created project
    public DateTime CreatedAt { get; set; }        // Creation timestamp
    public DateTime? UpdatedAt { get; set; }       // Last modification timestamp
}
```

### 2. Enums (`Enums/ProjectStatus.cs`)

**ProjectStatus** - Defines the lifecycle stages of a project:

- `PRE_PRODUCTION` - Initial planning stage (Default)
- `RECORDING` - Recording phase
- `MIXING` - Audio mixing stage
- `MASTERING` - Mastering stage
- `READY_FOR_REVIEW` - Ready for client review
- `RELEASED` - Project released
- `ARCHIVED` - Project archived

### 3. DTOs (Data Transfer Objects)

#### CreateProjectRequest (`DTOs/Projects/CreateProjectRequest.cs`)
Used to create a new project with validation:
```csharp
public class CreateProjectRequest
{
    [Required] [MaxLength(255)] public string Title { get; set; }
    [Required] [MaxLength(255)] public string ArtistName { get; set; }
    [MaxLength(1000)] public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? TargetReleaseDate { get; set; }
    [Required] public int CreatedBy { get; set; }
}
```

#### UpdateProjectRequest (`DTOs/Projects/UpdateProjectRequest.cs`)
Used to update an existing project with validation:
```csharp
public class UpdateProjectRequest
{
    [Required] [MaxLength(255)] public string Title { get; set; }
    [Required] [MaxLength(255)] public string ArtistName { get; set; }
    [MaxLength(1000)] public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? TargetReleaseDate { get; set; }
    public ProjectStatus? Status { get; set; }
}
```

#### PatchProjectRequest (`DTOs/Projects/PatchProjectRequest.cs`)
Used for partial updates of existing projects - **all fields are optional**:
```csharp
public class PatchProjectRequest
{
    [MaxLength(255)] public string? Title { get; set; }
    [MaxLength(255)] public string? ArtistName { get; set; }
    [MaxLength(1000)] public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? TargetReleaseDate { get; set; }
    public ProjectStatus? Status { get; set; }
}
```

#### ProjectResponse (`DTOs/Projects/ProjectResponse.cs`)
Response DTO containing all project data:
```csharp
public class ProjectResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ArtistName { get; set; }
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? TargetReleaseDate { get; set; }
    public ProjectStatus Status { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

### 4. Repository Layer

#### IProjectRepository (`Repositories/Interfaces/IProjectRepository.cs`)
Interface defining data access operations:
```csharp
public interface IProjectRepository
{
    Task<Project> CreateAsync(Project project);
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<Project> UpdateAsync(Project project);
    Task DeleteAsync(Project project);
}
```

#### ProjectRepository (`Repositories/ProjectRepository.cs`)
Concrete implementation of IProjectRepository with Entity Framework Core:
- **CreateAsync**: Adds and saves a new project
- **GetAllAsync**: Retrieves all projects ordered by creation date (newest first)
- **GetByIdAsync**: Finds a project by ID
- **UpdateAsync**: Updates an existing project
- **DeleteAsync**: Removes a project from database

### 5. Service Layer

#### IProjectService (`Services/Interfaces/IProjectService.cs`)
Interface defining business logic operations:
```csharp
public interface IProjectService
{
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request);
    Task<List<ProjectResponse>> GetAllAsync();
    Task<ProjectResponse> GetByIdAsync(int id);
    Task<ProjectResponse> UpdateAsync(int id, UpdateProjectRequest request);
    Task DeleteAsync(int id);
}
```

#### ProjectService (`Services/ProjectService.cs`)
Implements business logic with:
- Input validation and DTO mapping
- Exception handling (throws `ProjectNotFoundException` when project not found)
- Automatic timestamp management (UpdatedAt set on updates)
- Status update support
- Consistent data transformation through the `Map()` method

### 6. Controllers

#### ProjectsController (`Controllers/ProjectsController.cs`)
REST API endpoints for project management:

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/projects` | Create new project |
| GET | `/api/projects` | Get all projects |
| GET | `/api/projects/{id}` | Get project by ID |
| PUT | `/api/projects/{id}` | Update entire project |
| PATCH | `/api/projects/{id}` | Partial update (new) |
| DELETE | `/api/projects/{id}` | Delete project |

### 7. Exceptions

#### ProjectNotFoundException (`Exceptions/ProjectNotFoundException.cs`)
Custom exception thrown when a project is not found:
```csharp
public class ProjectNotFoundException : Exception
{
    public ProjectNotFoundException(int id)
        : base($"Project with id '{id}' not found.")
    {
    }
}
```

## Configuration

### Program.cs Setup
The Projects feature is configured in `Program.cs`:

```csharp
// Dependency injection
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();

// Database context includes Projects DbSet
public class AppDbContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
}
```

### Global Exception Handler
The exception handler in `Program.cs` catches `ProjectNotFoundException` and returns:
- **Status Code**: 404
- **Response**: `{ "error": "Project with id 'X' not found." }`

## Database Schema

### Projects Table
```sql
CREATE TABLE `Projects` (
    `Id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `Title` VARCHAR(255) NOT NULL,
    `ArtistName` VARCHAR(255) NOT NULL,
    `Description` VARCHAR(1000),
    `Deadline` DATETIME,
    `TargetReleaseDate` DATETIME,
    `Status` INT NOT NULL DEFAULT 0,
    `CreatedBy` INT NOT NULL,
    `CreatedAt` DATETIME NOT NULL,
    `UpdatedAt` DATETIME
)
```

## Migrations

- **Initial Migration**: `20260316021104_AddProjects.cs` - Creates the Projects table
- **Update Migration**: `AddUpdateAtToProjects.cs` - Adds the UpdatedAt field

## API Examples

### 1. Create Project
**Request:**
```http
POST /api/projects
Content-Type: application/json

{
    "title": "Summer Album 2026",
    "artistName": "The Composers",
    "description": "A summer-themed album collection",
    "deadline": "2026-06-30T00:00:00Z",
    "targetReleaseDate": "2026-07-15T00:00:00Z",
    "createdBy": 1
}
```

**Response (201 Created):**
```json
{
    "id": 1,
    "title": "Summer Album 2026",
    "artistName": "The Composers",
    "description": "A summer-themed album collection",
    "deadline": "2026-06-30T00:00:00Z",
    "targetReleaseDate": "2026-07-15T00:00:00Z",
    "status": "PRE_PRODUCTION",
    "createdBy": 1,
    "createdAt": "2026-03-16T10:30:00Z",
    "updatedAt": null
}
```

### 2. Get All Projects
**Request:**
```http
GET /api/projects
```

**Response (200 OK):**
```json
[
    {
        "id": 1,
        "title": "Summer Album 2026",
        "artistName": "The Composers",
        "status": "PRE_PRODUCTION",
        "createdAt": "2026-03-16T10:30:00Z",
        "updatedAt": null
    }
]
```

### 3. Get Project by ID
**Request:**
```http
GET /api/projects/1
```

**Response (200 OK):**
```json
{
    "id": 1,
    "title": "Summer Album 2026",
    "artistName": "The Composers",
    "description": "A summer-themed album collection",
    "deadline": "2026-06-30T00:00:00Z",
    "targetReleaseDate": "2026-07-15T00:00:00Z",
    "status": "PRE_PRODUCTION",
    "createdBy": 1,
    "createdAt": "2026-03-16T10:30:00Z",
    "updatedAt": null
}
```

### 4. Update Project
**Request:**
```http
PUT /api/projects/1
Content-Type: application/json

{
    "title": "Summer Album 2026 - Updated",
    "artistName": "The Composers",
    "description": "Updated description",
    "status": "RECORDING"
}
```

**Response (200 OK):**
```json
{
    "id": 1,
    "title": "Summer Album 2026 - Updated",
    "artistName": "The Composers",
    "description": "Updated description",
    "status": "RECORDING",
    "createdBy": 1,
    "createdAt": "2026-03-16T10:30:00Z",
    "updatedAt": "2026-03-16T11:45:00Z"
}
```

### 5. Patch Project (Partial Update)
**Request:**
```http
PATCH /api/projects/1
Content-Type: application/json

{
    "status": "MIXING",
    "description": "Now in mixing phase"
}
```

**Response (200 OK):**
```json
{
    "id": 1,
    "title": "Summer Album 2026 - Updated",
    "artistName": "The Composers",
    "description": "Now in mixing phase",
    "deadline": "2026-06-30T00:00:00Z",
    "targetReleaseDate": "2026-07-15T00:00:00Z",
    "status": "MIXING",
    "createdBy": 1,
    "createdAt": "2026-03-16T10:30:00Z",
    "updatedAt": "2026-03-16T12:00:00Z"
}
```

**Note**: Only send the fields you want to update. Other fields are preserved automatically.

### 6. Delete Project
**Request:**
```http
DELETE /api/projects/1
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

### Project Not Found
When trying to get, update, or delete a non-existent project:
```json
{
    "error": "Project with id '999' not found."
}
```

## Best Practices

1. **Always validate input** - Use Required, MaxLength, and other validation attributes
2. **Handle timestamps properly** - CreatedAt is set on creation, UpdatedAt on modification
3. **Use meaningful status transitions** - Follow the ProjectStatus enum lifecycle
4. **Check user permissions** - Verify that CreatedBy user exists before creation
5. **Return appropriate HTTP status codes**:
   - 201 Created for successful creation
   - 200 OK for successful read/update
   - 204 No Content for successful deletion
   - 400 Bad Request for validation errors
   - 404 Not Found for missing resources

## Future Enhancements

1. Add foreign key constraint to CreatedBy → Users table
2. Implement project collaboration (multiple users per project)
3. Add project tasks/milestones tracking
4. Implement project file attachments
5. Add project templates for quick setup
6. Implement project versioning/history
7. Add real-time project updates via SignalR
8. Implement project sharing and permissions

## Testing

To test the Projects feature:

1. **Create a project**: POST to `/api/projects` with required fields
2. **List all projects**: GET `/api/projects`
3. **Get specific project**: GET `/api/projects/{id}`
4. **Update project status**: PUT `/api/projects/{id}` with new status
5. **Delete project**: DELETE `/api/projects/{id}`

See `Docs/AUTH_TESTING.md` for detailed testing examples using Postman or similar tools.

## Summary

The Projects feature is fully implemented with:
- ✅ Complete CRUD operations
- ✅ Input validation with Data Annotations
- ✅ Proper exception handling
- ✅ Timestamp tracking (CreatedAt, UpdatedAt)
- ✅ Project status lifecycle management
- ✅ Clean layered architecture
- ✅ Dependency injection
- ✅ Entity Framework Core integration
- ✅ Database migrations
- ✅ Global exception handling

