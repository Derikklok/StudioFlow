# PATCH Request - Partial Updates Documentation

## Overview

The PATCH endpoint allows you to perform **partial updates** on existing projects without requiring all fields like PUT does.

### Key Differences: PATCH vs PUT

| Aspect | PATCH | PUT |
|--------|-------|-----|
| **Update Type** | Partial | Complete |
| **Required Fields** | Only fields you want to update | All required fields |
| **Use Case** | Update specific fields | Replace entire resource |
| **Fields** | All optional | Title, ArtistName required |

---

## API Endpoint

### PATCH /api/projects/{id}

Update specific fields of a project. Only send the fields you want to change.

**Method**: `PATCH`

**URL**: `http://localhost:5000/api/projects/{id}`

**Content-Type**: `application/json`

---

## PATCH Request Examples

### Example 1: Update Only Status

Change project status without modifying other fields.

```http
PATCH /api/projects/1
Content-Type: application/json

{
    "status": "RECORDING"
}
```

**Response (200 OK)**:
```json
{
    "id": 1,
    "title": "Summer Album 2026",
    "artistName": "The Composers",
    "description": "A summer-themed album collection",
    "deadline": "2026-06-30T00:00:00Z",
    "targetReleaseDate": "2026-07-15T00:00:00Z",
    "status": "RECORDING",
    "createdBy": 1,
    "createdAt": "2026-03-16T12:30:45.123456Z",
    "updatedAt": "2026-03-16T14:00:00.654321Z"
}
```

**Note**: Only `status` was updated, all other fields remain unchanged. `updatedAt` is automatically set.

---

### Example 2: Update Multiple Specific Fields

Update description and deadline while keeping other fields intact.

```http
PATCH /api/projects/1
Content-Type: application/json

{
    "description": "Updated: Now in mixing phase",
    "deadline": "2026-07-10T00:00:00Z"
}
```

**Response (200 OK)**:
```json
{
    "id": 1,
    "title": "Summer Album 2026",
    "artistName": "The Composers",
    "description": "Updated: Now in mixing phase",
    "deadline": "2026-07-10T00:00:00Z",
    "targetReleaseDate": "2026-07-15T00:00:00Z",
    "status": "RECORDING",
    "createdBy": 1,
    "createdAt": "2026-03-16T12:30:45.123456Z",
    "updatedAt": "2026-03-16T14:05:30.789012Z"
}
```

---

### Example 3: Update Title and Artist Name

Update project title and artist name.

```http
PATCH /api/projects/1
Content-Type: application/json

{
    "title": "Summer Album 2026 - Final Mix",
    "artistName": "The Composers & Friends"
}
```

**Response (200 OK)**:
```json
{
    "id": 1,
    "title": "Summer Album 2026 - Final Mix",
    "artistName": "The Composers & Friends",
    "description": "A summer-themed album collection",
    "deadline": "2026-06-30T00:00:00Z",
    "targetReleaseDate": "2026-07-15T00:00:00Z",
    "status": "RECORDING",
    "createdBy": 1,
    "createdAt": "2026-03-16T12:30:45.123456Z",
    "updatedAt": "2026-03-16T14:10:15.234567Z"
}
```

---

### Example 4: Clear Description (Set to Empty)

Remove or clear the description by sending an empty string or null.

```http
PATCH /api/projects/1
Content-Type: application/json

{
    "description": ""
}
```

**Response (200 OK)**:
```json
{
    "id": 1,
    "title": "Summer Album 2026",
    "artistName": "The Composers",
    "description": "",
    "deadline": "2026-06-30T00:00:00Z",
    "targetReleaseDate": "2026-07-15T00:00:00Z",
    "status": "RECORDING",
    "createdBy": 1,
    "createdAt": "2026-03-16T12:30:45.123456Z",
    "updatedAt": "2026-03-16T14:15:00.345678Z"
}
```

---

### Example 5: Multiple Status Updates

Progress through project lifecycle.

**Step 1: Move to Mixing**
```http
PATCH /api/projects/1
Content-Type: application/json

{
    "status": "MIXING"
}
```

**Step 2: Move to Mastering**
```http
PATCH /api/projects/1
Content-Type: application/json

{
    "status": "MASTERING"
}
```

**Step 3: Move to Ready for Review**
```http
PATCH /api/projects/1
Content-Type: application/json

{
    "status": "READY_FOR_REVIEW"
}
```

**Step 4: Release**
```http
PATCH /api/projects/1
Content-Type: application/json

{
    "status": "RELEASED"
}
```

---

## Valid Fields for PATCH

All of these fields are optional and can be updated individually:

| Field | Type | Max Length | Description |
|-------|------|-----------|-------------|
| `title` | String | 255 | Project title |
| `artistName` | String | 255 | Artist/Band name |
| `description` | String | 1000 | Project description |
| `deadline` | DateTime | N/A | Project deadline |
| `targetReleaseDate` | DateTime | N/A | Target release date |
| `status` | Enum | N/A | Project status (PRE_PRODUCTION, RECORDING, MIXING, MASTERING, READY_FOR_REVIEW, RELEASED, ARCHIVED) |

---

## Validation Rules

Even for PATCH requests, validation rules apply to the fields you send:

```http
PATCH /api/projects/1
Content-Type: application/json

{
    "title": "A".repeat(300)
}
```

**Response (400 Bad Request)**:
```json
{
    "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "errors": {
        "Title": ["Title cannot exceed 255 characters"]
    }
}
```

---

## Error Responses

### Project Not Found

```http
PATCH /api/projects/999
```

**Response (404 Not Found)**:
```json
{
    "error": "Project with id '999' not found."
}
```

---

### Empty Request Body

```http
PATCH /api/projects/1
Content-Type: application/json

{}
```

**Response (200 OK)**: Project returned unchanged with `updatedAt` updated to current time.

```json
{
    "id": 1,
    "title": "Summer Album 2026",
    "artistName": "The Composers",
    "description": "A summer-themed album collection",
    "deadline": "2026-06-30T00:00:00Z",
    "targetReleaseDate": "2026-07-15T00:00:00Z",
    "status": "RECORDING",
    "createdBy": 1,
    "createdAt": "2026-03-16T12:30:45.123456Z",
    "updatedAt": "2026-03-16T14:20:00.456789Z"
}
```

---

## PATCH vs PUT Comparison

### Using PUT (Complete Update)

Requires all fields to be sent:

```http
PUT /api/projects/1
Content-Type: application/json

{
    "title": "Updated Title",
    "artistName": "Updated Artist",
    "description": "Updated description",
    "deadline": "2026-07-10T00:00:00Z"
}
```

All required fields must be present or you get a 400 error.

---

### Using PATCH (Partial Update)

Only send what you want to change:

```http
PATCH /api/projects/1
Content-Type: application/json

{
    "status": "MIXING"
}
```

Other fields are preserved automatically.

---

## Best Practices

1. **Use PATCH for single field updates**: More efficient than PUT
   ```json
   { "status": "RECORDING" }
   ```

2. **Use PUT for major updates**: When replacing multiple core fields
   ```json
   {
       "title": "New Title",
       "artistName": "New Artist",
       "description": "New description"
   }
   ```

3. **Always check the response**: To verify all fields after update

4. **Monitor updatedAt**: Check this timestamp to verify the update was applied

5. **Validate before sending**: Check field lengths against max limits

---

## Postman Examples

### Collection Variable Setup

```javascript
// Pre-request script
pm.environment.set("project_id", 1);
```

### PATCH - Update Status Only

```javascript
// Test
pm.test("Status updated successfully", function() {
    var jsonData = pm.response.json();
    pm.expect(jsonData.status).to.eql("RECORDING");
    pm.expect(jsonData.updatedAt).to.not.be.null;
});
```

### PATCH - Multiple Fields

```javascript
// Test
pm.test("Multiple fields updated", function() {
    var jsonData = pm.response.json();
    pm.expect(jsonData.title).to.include("Updated");
    pm.expect(jsonData.status).to.eql("MIXING");
});
```

---

## HTTP Status Codes

| Status | Meaning |
|--------|---------|
| 200 | OK - Successful partial update |
| 400 | Bad Request - Validation error |
| 404 | Not Found - Project doesn't exist |
| 500 | Internal Server Error - Unexpected error |

---

## Summary

PATCH requests provide a convenient way to:
- ✅ Update single fields without sending entire object
- ✅ Reduce request payload size
- ✅ Progress through project lifecycle easily
- ✅ Apply quick fixes to specific project details
- ✅ Maintain cleaner API usage patterns

The PATCH endpoint is fully integrated with the Projects API and follows RESTful best practices.

