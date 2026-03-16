# Samples Feature - Quick Reference Guide

## Feature Status: ✅ COMPLETE AND PRODUCTION-READY

---

## Quick Start

### 1. Ensure Database is Migrated
```bash
dotnet ef database update
```

### 2. Start the Application
```bash
dotnet run
```

### 3. Test the API
Use Postman, Insomnia, or curl to test endpoints below.

---

## Core Endpoints

### Create Sample
```
POST /api/projects/{projectId}/samples
Content-Type: application/json

{
  "title": "Sample Title",
  "sourceArtist": "Artist Name",
  "sourceTrack": "Track Name",
  "rightsHolder": "Holder Name"
}

Response: 201 Created
```

### Get Samples for Project
```
GET /api/projects/{projectId}/samples

Response: 200 OK
Returns: Array of samples
```

### Get Sample by ID
```
GET /api/samples/{id}

Response: 200 OK
Returns: Single sample
```

### Update Sample (Full)
```
PUT /api/samples/{id}
Content-Type: application/json

{
  "title": "Updated Title",
  "sourceArtist": "Artist",
  "sourceTrack": "Track",
  "rightsHolder": "Holder",
  "status": "PENDING_CLEARENCE"
}

Response: 200 OK
```

### Partial Update Sample
```
PATCH /api/samples/{id}
Content-Type: application/json

{
  "status": "APPROVED"
}

Response: 200 OK
(Only specify fields to update)
```

### Delete Sample
```
DELETE /api/samples/{id}

Response: 204 No Content
```

---

## Sample Status Values

```
DRAFT              - Default, sample is in draft
PENDING_CLEARENCE  - Awaiting approval/clearance
APPROVED           - Sample is approved
REJECTED           - Sample is rejected
```

---

## Common Error Responses

### Invalid Project
```
404 Not Found
{
  "error": "Project with id 'X' not found."
}
```

### Invalid Sample
```
404 Not Found
{
  "error": "Sample with id 'X' not found."
}
```

### Validation Error
```
400 Bad Request
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": { ... }
}
```

---

## Field Validation

| Field | Type | Max Length | Required |
|-------|------|-----------|----------|
| Title | string | 255 | ✅ Yes |
| SourceArtist | string | 255 | ❌ No |
| SourceTrack | string | 255 | ❌ No |
| RightsHolder | string | 255 | ❌ No |
| Status | enum | - | ❌ No (default: DRAFT) |

---

## Database Relationships

```
Project (1)
  ↓
  └─→ (Many) Sample

When a Project is deleted:
- All associated Samples are automatically deleted (Cascade)
```

---

## Complete CRUD Example

### Step 1: Create Project (Pre-requisite)
```bash
curl -X POST http://localhost:5000/api/projects \
  -H "Content-Type: application/json" \
  -d '{
    "title": "My Album",
    "artistName": "My Artist",
    "createdBy": 1
  }'
```

### Step 2: Create Sample
```bash
curl -X POST http://localhost:5000/api/projects/1/samples \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Jazz Intro",
    "sourceArtist": "Jazz Masters"
  }'
```

### Step 3: Read Sample
```bash
curl -X GET http://localhost:5000/api/samples/1
```

### Step 4: Update Sample
```bash
curl -X PUT http://localhost:5000/api/samples/1 \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Jazz Intro - Extended",
    "sourceArtist": "Jazz Masters",
    "sourceTrack": "Blue Horizon",
    "rightsHolder": "Jazz Masters LLC",
    "status": "PENDING_CLEARENCE"
  }'
```

### Step 5: Partial Update
```bash
curl -X PATCH http://localhost:5000/api/samples/1 \
  -H "Content-Type: application/json" \
  -d '{
    "status": "APPROVED"
  }'
```

### Step 6: Delete Sample
```bash
curl -X DELETE http://localhost:5000/api/samples/1
```

---

## Implementation Architecture

```
SamplesController
      ↓
   ISampleService
      ↓
   SampleService
      ↓
   ISampleRepository
      ↓
   SampleRepository
      ↓
   AppDbContext
      ↓
   Database (MySQL)
```

---

## Key Features

✅ Full CRUD Operations
✅ Status Workflow Support
✅ Timestamp Tracking (CreatedAt, UpdatedAt)
✅ Cascade Delete on Project Deletion
✅ Input Validation
✅ Proper Error Handling
✅ RESTful API Design
✅ Partial Update Support (PATCH)

---

## Files Changed

### Modified
- `Models/Project.cs` - Added Samples navigation
- `Data/AppDbContext.cs` - Added relationship config

### Created (Documentation)
- `Docs/SAMPLES_IMPLEMENTATION.md`
- `Docs/SAMPLES_FIXES_REPORT.md`
- `Docs/SAMPLES_TESTING_GUIDE.md`
- `Docs/SAMPLES_COMPLETE_VERIFICATION.md`
- `Docs/SAMPLES_QUICK_REFERENCE.md` (this file)

---

## Troubleshooting

### Issue: ProjectNotFoundException
**Solution**: Ensure project exists. Create a project first using Projects API.

### Issue: Validation Error
**Solution**: Check field lengths and required fields match documentation.

### Issue: UpdatedAt Not Showing
**Solution**: UpdatedAt is null for new samples. It updates when you modify them.

---

## Performance Tips

1. **Filter by Project**: Use GET /api/projects/{projectId}/samples for faster queries
2. **Bulk Operations**: Consider adding batch create/delete in future
3. **Caching**: Not implemented yet, consider for high traffic
4. **Indexing**: Database indexes already on ProjectId

---

## Testing Tools

- **Postman**: https://www.postman.com/
- **Insomnia**: https://insomnia.rest/
- **curl**: Command line tool
- **VS Code REST Client**: Extension for VS Code

---

## Documentation Files

| Document | Purpose |
|----------|---------|
| SAMPLES_IMPLEMENTATION.md | Architecture and design details |
| SAMPLES_TESTING_GUIDE.md | Comprehensive test scenarios |
| SAMPLES_FIXES_REPORT.md | Issues found and fixed |
| SAMPLES_COMPLETE_VERIFICATION.md | Full verification checklist |
| SAMPLES_QUICK_REFERENCE.md | This quick reference |

---

## Build & Deployment

### Build
```bash
dotnet build
```

### Build Release
```bash
dotnet build --configuration Release
```

### Run
```bash
dotnet run
```

### Run Release
```bash
dotnet run --configuration Release
```

---

## Database Commands

### Check Samples Table
```sql
SELECT * FROM Samples;
```

### Check Samples for Project
```sql
SELECT * FROM Samples WHERE ProjectId = 1;
```

### Count Samples
```sql
SELECT COUNT(*) FROM Samples;
```

### Check Relationships
```sql
SELECT * FROM Samples 
INNER JOIN Projects ON Samples.ProjectId = Projects.Id 
WHERE Projects.Id = 1;
```

---

## Important Notes

1. **ProjectId is Required**: Every sample must belong to a project
2. **Cascade Delete**: Deleting a project deletes all its samples
3. **Timestamps are UTC**: All times stored in UTC format
4. **Status is Flexible**: Can change between any states
5. **Optional Fields**: Only Title is required, others are optional

---

## Next Features to Implement

- [ ] Sample file attachments
- [ ] Sample preview/playback
- [ ] Sample duration tracking
- [ ] Clearance deadline tracking
- [ ] Batch operations
- [ ] Usage history/audit trail
- [ ] Licensing information
- [ ] Quality ratings
- [ ] Tagging/categorization

---

## Support & Documentation

For detailed information:
1. Read SAMPLES_IMPLEMENTATION.md for architecture
2. Follow SAMPLES_TESTING_GUIDE.md for testing
3. Check SAMPLES_COMPLETE_VERIFICATION.md for verification
4. Review code comments for specific implementations

---

## Version Info

- **Feature**: Samples CRUD
- **Status**: ✅ Complete
- **Date**: March 16, 2026
- **Version**: 1.0
- **Ready**: ✅ Production

---

Last Updated: March 16, 2026

