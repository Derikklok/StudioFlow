# Samples Feature - Testing Guide

## Prerequisites

1. Application is running (dotnet run)
2. Database is migrated and contains at least one project
3. Postman or similar API testing tool installed

---

## Test Sequence

### Step 1: Create a Project (Pre-requisite)

**Endpoint**: `POST /api/projects`

**Request**:
```json
{
    "title": "Summer Album 2026",
    "artistName": "The Jazz Quartet",
    "description": "A summer-themed jazz album",
    "deadline": "2026-06-30T00:00:00Z",
    "targetReleaseDate": "2026-07-15T00:00:00Z",
    "createdBy": 1
}
```

**Expected Response**: 201 Created
```json
{
    "id": 1,
    "title": "Summer Album 2026",
    "artistName": "The Jazz Quartet",
    "description": "A summer-themed jazz album",
    "deadline": "2026-06-30T00:00:00Z",
    "targetReleaseDate": "2026-07-15T00:00:00Z",
    "status": "PRE_PRODUCTION",
    "createdBy": 1,
    "createdAt": "2026-03-16T10:30:00Z",
    "updatedAt": null
}
```

**Note**: Save the project ID (1) for use in subsequent tests.

---

## Test Group 1: CREATE Operations

### Test 1.1: Create Sample - Valid Request

**Endpoint**: `POST /api/projects/1/samples`

**Request Body**:
```json
{
    "title": "Smooth Jazz Intro",
    "sourceArtist": "Jazz Masters",
    "sourceTrack": "Blue Horizon",
    "rightsHolder": "Jazz Masters LLC"
}
```

**Expected Response**: 201 Created
```json
{
    "id": 1,
    "projectId": 1,
    "title": "Smooth Jazz Intro",
    "sourceArtist": "Jazz Masters",
    "sourceTrack": "Blue Horizon",
    "rightsHolder": "Jazz Masters LLC",
    "status": "DRAFT",
    "createdAt": "2026-03-16T10:35:00Z",
    "updatedAt": null
}
```

**Validation**:
- ✅ Sample created successfully
- ✅ Status defaults to DRAFT
- ✅ CreatedAt is automatically set
- ✅ UpdatedAt is null for new sample

---

### Test 1.2: Create Sample - Without Optional Fields

**Endpoint**: `POST /api/projects/1/samples`

**Request Body**:
```json
{
    "title": "Electronic Pad"
}
```

**Expected Response**: 201 Created
```json
{
    "id": 2,
    "projectId": 1,
    "title": "Electronic Pad",
    "sourceArtist": null,
    "sourceTrack": null,
    "rightsHolder": null,
    "status": "DRAFT",
    "createdAt": "2026-03-16T10:36:00Z",
    "updatedAt": null
}
```

**Validation**:
- ✅ Optional fields can be omitted
- ✅ Null values handled correctly

---

### Test 1.3: Create Sample - Missing Required Field

**Endpoint**: `POST /api/projects/1/samples`

**Request Body**:
```json
{
    "sourceArtist": "Artist Name"
}
```

**Expected Response**: 400 Bad Request
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

**Validation**:
- ✅ Validation error properly returned
- ✅ Error message is clear

---

### Test 1.4: Create Sample - Invalid Project ID

**Endpoint**: `POST /api/projects/999/samples`

**Request Body**:
```json
{
    "title": "Sample Title"
}
```

**Expected Response**: 404 Not Found
```json
{
    "error": "Project with id '999' not found."
}
```

**Validation**:
- ✅ Project existence validated
- ✅ Proper error message returned

---

## Test Group 2: READ Operations

### Test 2.1: Get All Samples for Project

**Endpoint**: `GET /api/projects/1/samples`

**Expected Response**: 200 OK
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
        "createdAt": "2026-03-16T10:35:00Z",
        "updatedAt": null
    },
    {
        "id": 2,
        "projectId": 1,
        "title": "Electronic Pad",
        "sourceArtist": null,
        "sourceTrack": null,
        "rightsHolder": null,
        "status": "DRAFT",
        "createdAt": "2026-03-16T10:36:00Z",
        "updatedAt": null
    }
]
```

**Validation**:
- ✅ All samples returned
- ✅ Ordered by creation date (newest first)
- ✅ Correct structure for each sample

---

### Test 2.2: Get Sample by ID

**Endpoint**: `GET /api/samples/1`

**Expected Response**: 200 OK
```json
{
    "id": 1,
    "projectId": 1,
    "title": "Smooth Jazz Intro",
    "sourceArtist": "Jazz Masters",
    "sourceTrack": "Blue Horizon",
    "rightsHolder": "Jazz Masters LLC",
    "status": "DRAFT",
    "createdAt": "2026-03-16T10:35:00Z",
    "updatedAt": null
}
```

**Validation**:
- ✅ Correct sample returned
- ✅ All fields properly populated

---

### Test 2.3: Get Non-existent Sample

**Endpoint**: `GET /api/samples/999`

**Expected Response**: 404 Not Found
```json
{
    "error": "Sample with id '999' not found."
}
```

**Validation**:
- ✅ 404 error returned
- ✅ Clear error message

---

## Test Group 3: UPDATE Operations (Full Update)

### Test 3.1: Update Sample - All Fields

**Endpoint**: `PUT /api/samples/1`

**Request Body**:
```json
{
    "title": "Smooth Jazz Intro - Extended Version",
    "sourceArtist": "Jazz Masters Ensemble",
    "sourceTrack": "Blue Horizon - Full Length",
    "rightsHolder": "Jazz Masters LLC - 2026",
    "status": "PENDING_CLEARENCE"
}
```

**Expected Response**: 200 OK
```json
{
    "id": 1,
    "projectId": 1,
    "title": "Smooth Jazz Intro - Extended Version",
    "sourceArtist": "Jazz Masters Ensemble",
    "sourceTrack": "Blue Horizon - Full Length",
    "rightsHolder": "Jazz Masters LLC - 2026",
    "status": "PENDING_CLEARENCE",
    "createdAt": "2026-03-16T10:35:00Z",
    "updatedAt": "2026-03-16T11:00:00Z"
}
```

**Validation**:
- ✅ All fields updated correctly
- ✅ CreatedAt unchanged
- ✅ UpdatedAt set to current time
- ✅ Status changed successfully

---

### Test 3.2: Update Sample - Status Change

**Endpoint**: `PUT /api/samples/1`

**Request Body**:
```json
{
    "title": "Smooth Jazz Intro - Extended Version",
    "sourceArtist": "Jazz Masters Ensemble",
    "sourceTrack": "Blue Horizon - Full Length",
    "rightsHolder": "Jazz Masters LLC - 2026",
    "status": "APPROVED"
}
```

**Expected Response**: 200 OK
```json
{
    "id": 1,
    "projectId": 1,
    "title": "Smooth Jazz Intro - Extended Version",
    "sourceArtist": "Jazz Masters Ensemble",
    "sourceTrack": "Blue Horizon - Full Length",
    "rightsHolder": "Jazz Masters LLC - 2026",
    "status": "APPROVED",
    "createdAt": "2026-03-16T10:35:00Z",
    "updatedAt": "2026-03-16T11:05:00Z"
}
```

**Validation**:
- ✅ Status transitioned to APPROVED
- ✅ UpdatedAt updated
- ✅ Other fields preserved

---

### Test 3.3: Update Non-existent Sample

**Endpoint**: `PUT /api/samples/999`

**Request Body**:
```json
{
    "title": "Any Title",
    "sourceArtist": "Any Artist",
    "sourceTrack": "Any Track",
    "rightsHolder": "Any Holder",
    "status": "DRAFT"
}
```

**Expected Response**: 404 Not Found
```json
{
    "error": "Sample with id '999' not found."
}
```

**Validation**:
- ✅ 404 error returned
- ✅ No record created/modified

---

## Test Group 4: PATCH Operations (Partial Update)

### Test 4.1: Patch Sample - Status Only

**Endpoint**: `PATCH /api/samples/1`

**Request Body**:
```json
{
    "status": "REJECTED"
}
```

**Expected Response**: 200 OK
```json
{
    "id": 1,
    "projectId": 1,
    "title": "Smooth Jazz Intro - Extended Version",
    "sourceArtist": "Jazz Masters Ensemble",
    "sourceTrack": "Blue Horizon - Full Length",
    "rightsHolder": "Jazz Masters LLC - 2026",
    "status": "REJECTED",
    "createdAt": "2026-03-16T10:35:00Z",
    "updatedAt": "2026-03-16T11:10:00Z"
}
```

**Validation**:
- ✅ Only status updated
- ✅ Other fields unchanged
- ✅ UpdatedAt refreshed

---

### Test 4.2: Patch Sample - Title Only

**Endpoint**: `PATCH /api/samples/2`

**Request Body**:
```json
{
    "title": "Electronic Ambient Pad - New Title"
}
```

**Expected Response**: 200 OK
```json
{
    "id": 2,
    "projectId": 1,
    "title": "Electronic Ambient Pad - New Title",
    "sourceArtist": null,
    "sourceTrack": null,
    "rightsHolder": null,
    "status": "DRAFT",
    "createdAt": "2026-03-16T10:36:00Z",
    "updatedAt": "2026-03-16T11:15:00Z"
}
```

**Validation**:
- ✅ Title updated correctly
- ✅ Null fields remain null
- ✅ Status unchanged

---

### Test 4.3: Patch Sample - Multiple Fields

**Endpoint**: `PATCH /api/samples/2`

**Request Body**:
```json
{
    "sourceArtist": "Electronic Dreams",
    "sourceTrack": "Ambient Atmosphere",
    "status": "PENDING_CLEARENCE"
}
```

**Expected Response**: 200 OK
```json
{
    "id": 2,
    "projectId": 1,
    "title": "Electronic Ambient Pad - New Title",
    "sourceArtist": "Electronic Dreams",
    "sourceTrack": "Ambient Atmosphere",
    "rightsHolder": null,
    "status": "PENDING_CLEARENCE",
    "createdAt": "2026-03-16T10:36:00Z",
    "updatedAt": "2026-03-16T11:20:00Z"
}
```

**Validation**:
- ✅ Multiple fields updated
- ✅ Unspecified fields preserved
- ✅ UpdatedAt refreshed

---

### Test 4.4: Patch Empty Body

**Endpoint**: `PATCH /api/samples/1`

**Request Body**:
```json
{}
```

**Expected Response**: 200 OK
```json
{
    "id": 1,
    "projectId": 1,
    "title": "Smooth Jazz Intro - Extended Version",
    "sourceArtist": "Jazz Masters Ensemble",
    "sourceTrack": "Blue Horizon - Full Length",
    "rightsHolder": "Jazz Masters LLC - 2026",
    "status": "REJECTED",
    "createdAt": "2026-03-16T10:35:00Z",
    "updatedAt": "2026-03-16T11:10:00Z"
}
```

**Validation**:
- ✅ No changes applied
- ✅ UpdatedAt still updated (by service logic)
- ✅ No errors

---

## Test Group 5: DELETE Operations

### Test 5.1: Delete Sample

**Endpoint**: `DELETE /api/samples/2`

**Expected Response**: 204 No Content
```
(Empty body)
```

**Validation**:
- ✅ No response body
- ✅ Status code 204

---

### Test 5.2: Verify Sample Deleted

**Endpoint**: `GET /api/samples/2`

**Expected Response**: 404 Not Found
```json
{
    "error": "Sample with id '2' not found."
}
```

**Validation**:
- ✅ Sample no longer exists
- ✅ Proper 404 error

---

### Test 5.3: Delete Non-existent Sample

**Endpoint**: `DELETE /api/samples/999`

**Expected Response**: 404 Not Found
```json
{
    "error": "Sample with id '999' not found."
}
```

**Validation**:
- ✅ 404 error returned
- ✅ No side effects

---

## Test Group 6: Cascade Delete (Advanced)

### Test 6.1: Create New Project with Sample

**Endpoint 1**: `POST /api/projects`
```json
{
    "title": "Test Project for Cascade",
    "artistName": "Test Artist",
    "createdBy": 1
}
```

**Save the project ID** (should be 2)

**Endpoint 2**: `POST /api/projects/2/samples`
```json
{
    "title": "Test Sample for Cascade Delete"
}
```

**Save the sample ID** (should be 3)

---

### Test 6.2: Delete Project and Verify Samples Deleted

**Endpoint**: `DELETE /api/projects/2`

**Expected Response**: 204 No Content

**Then GET Sample**:
```
GET /api/samples/3
```

**Expected Response**: 404 Not Found
```json
{
    "error": "Sample with id '3' not found."
}
```

**Validation**:
- ✅ Sample automatically deleted when project deleted
- ✅ Cascade delete working correctly

---

## Test Group 7: Status Workflow Tests

### Test 7.1: DRAFT → PENDING_CLEARENCE

**Create Sample** → Status: DRAFT

**Patch to PENDING_CLEARENCE**:
```json
{
    "status": "PENDING_CLEARENCE"
}
```

**Validation**: ✅ Status transition successful

---

### Test 7.2: PENDING_CLEARENCE → APPROVED

**Current Status**: PENDING_CLEARENCE

**Patch to APPROVED**:
```json
{
    "status": "APPROVED"
}
```

**Validation**: ✅ Status transition successful

---

### Test 7.3: APPROVED → REJECTED

**Current Status**: APPROVED

**Patch to REJECTED**:
```json
{
    "status": "REJECTED"
}
```

**Validation**: ✅ Status transition successful

---

## Test Group 8: Validation Tests

### Test 8.1: Title Too Long

**Endpoint**: `POST /api/projects/1/samples`

**Request Body**:
```json
{
    "title": "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
}
```

**Expected Response**: 400 Bad Request
```json
{
    "errors": {
        "Title": ["Title cannot exceed 255 characters"]
    }
}
```

---

### Test 8.2: Field Length Validation

**Endpoint**: `POST /api/projects/1/samples`

**Request Body**:
```json
{
    "title": "Valid Title",
    "sourceArtist": "x".repeat(300)
}
```

**Expected Response**: 400 Bad Request
```json
{
    "errors": {
        "SourceArtist": ["SourceArtist cannot exceed 255 characters"]
    }
}
```

---

## Performance Tests

### Test P.1: Create Multiple Samples

Create 100 samples for the same project and measure response times.

**Expected**: Each request completes in < 500ms

---

### Test P.2: List Large Sample Collection

Create 100 samples and retrieve them all.

**Expected**: GET completes in < 1000ms

---

## Summary Checklist

- [ ] All CREATE tests passed
- [ ] All READ tests passed
- [ ] All UPDATE (PUT) tests passed
- [ ] All PATCH tests passed
- [ ] All DELETE tests passed
- [ ] Cascade delete working
- [ ] Status workflow validated
- [ ] Validation tests passed
- [ ] Performance tests passed
- [ ] No console errors
- [ ] No database errors

---

## Troubleshooting

### Issue: ProjectNotFoundException when creating sample
**Solution**: Verify project ID exists. Use GET /api/projects to list existing projects.

### Issue: SampleNotFoundException on DELETE
**Solution**: Verify sample ID exists. Use GET /api/projects/{projectId}/samples to list samples.

### Issue: Validation errors on valid input
**Solution**: Check field lengths and required fields. Review error message for specific field.

### Issue: UpdatedAt not changing on PATCH
**Solution**: This is expected behavior - check that the service is being called (should see timestamp update).

---

## Database Verification

### Check Created Samples
```sql
SELECT * FROM Samples WHERE ProjectId = 1;
```

### Check Sample Count
```sql
SELECT COUNT(*) FROM Samples;
```

### Verify Foreign Key
```sql
SELECT * FROM Samples WHERE ProjectId = 999;
```
(Should return 0 rows)

---

## Conclusion

This comprehensive test suite covers:
- ✅ CRUD operations
- ✅ Validation
- ✅ Error handling
- ✅ Relationships
- ✅ Cascade deletes
- ✅ Status workflows
- ✅ Performance

All tests should pass successfully with the implemented Samples feature.

