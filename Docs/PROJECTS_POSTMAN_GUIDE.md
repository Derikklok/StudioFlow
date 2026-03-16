# Projects API - Testing Guide (Post-Migration)

## ✅ Database Updated

The migration has been successfully applied. The `UpdatedAt` column now exists in the Projects table.

---

## 🧪 Testing the Projects API

### 1. Create a Project (POST)

**URL**: `http://localhost:5000/api/projects` (adjust port if needed)

**Method**: `POST`

**Headers**:
```
Content-Type: application/json
```

**Request Body**:
```json
{
    "title": "Summer Album 2026",
    "artistName": "The Composers",
    "description": "A summer-themed album collection",
    "deadline": "2026-06-30T00:00:00Z",
    "targetReleaseDate": "2026-07-15T00:00:00Z",
    "createdBy": 1
}
```

**Expected Response (201 Created)**:
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
    "createdAt": "2026-03-16T12:30:45.123456Z",
    "updatedAt": null
}
```

---

### 2. Get All Projects (GET)

**URL**: `http://localhost:5000/api/projects`

**Method**: `GET`

**Expected Response (200 OK)**:
```json
[
    {
        "id": 1,
        "title": "Summer Album 2026",
        "artistName": "The Composers",
        "description": "A summer-themed album collection",
        "deadline": "2026-06-30T00:00:00Z",
        "targetReleaseDate": "2026-07-15T00:00:00Z",
        "status": "PRE_PRODUCTION",
        "createdBy": 1,
        "createdAt": "2026-03-16T12:30:45.123456Z",
        "updatedAt": null
    }
]
```

---

### 3. Get Single Project (GET)

**URL**: `http://localhost:5000/api/projects/1`

**Method**: `GET`

**Expected Response (200 OK)**:
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
    "createdAt": "2026-03-16T12:30:45.123456Z",
    "updatedAt": null
}
```

---

### 4. Update Project (PUT)

**URL**: `http://localhost:5000/api/projects/1`

**Method**: `PUT`

**Headers**:
```
Content-Type: application/json
```

**Request Body**:
```json
{
    "title": "Summer Album 2026 - Updated",
    "artistName": "The Composers",
    "description": "Updated description - now recording!",
    "status": "RECORDING"
}
```

**Expected Response (200 OK)**:
```json
{
    "id": 1,
    "title": "Summer Album 2026 - Updated",
    "artistName": "The Composers",
    "description": "Updated description - now recording!",
    "deadline": "2026-06-30T00:00:00Z",
    "targetReleaseDate": "2026-07-15T00:00:00Z",
    "status": "RECORDING",
    "createdBy": 1,
    "createdAt": "2026-03-16T12:30:45.123456Z",
    "updatedAt": "2026-03-16T12:45:30.654321Z"
}
```

**Notice**: `updatedAt` is now set to the current timestamp!

---

### 5. Patch Project (PATCH)

**URL**: `http://localhost:5000/api/projects/1`

**Method**: `PATCH`

**Headers**:
```
Content-Type: application/json
```

**Request Body** (Update only status):
```json
{
    "status": "RECORDING"
}
```

**Expected Response (200 OK)**:
```json
{
    "id": 1,
    "title": "Summer Album 2026 - Updated",
    "artistName": "The Composers",
    "description": "Updated description - now recording!",
    "deadline": "2026-06-30T00:00:00Z",
    "targetReleaseDate": "2026-07-15T00:00:00Z",
    "status": "RECORDING",
    "createdBy": 1,
    "createdAt": "2026-03-16T12:30:45.123456Z",
    "updatedAt": "2026-03-16T12:45:30.654321Z"
}
```

**Note**: PATCH allows partial updates. Only send the fields you want to change!

---

### 5b. Patch - Multiple Fields

**URL**: `http://localhost:5000/api/projects/1`

**Method**: `PATCH`

**Request Body** (Update multiple specific fields):
```json
{
    "title": "Summer Album 2026 - Final Mix",
    "description": "Ready for final mixing phase",
    "status": "MIXING"
}
```

**Expected Response (200 OK)**:
```json
{
    "id": 1,
    "title": "Summer Album 2026 - Final Mix",
    "artistName": "The Composers",
    "description": "Ready for final mixing phase",
    "deadline": "2026-06-30T00:00:00Z",
    "targetReleaseDate": "2026-07-15T00:00:00Z",
    "status": "MIXING",
    "createdBy": 1,
    "createdAt": "2026-03-16T12:30:45.123456Z",
    "updatedAt": "2026-03-16T13:00:00.789012Z"
}
```

---

### 6. Delete Project (DELETE)

**URL**: `http://localhost:5000/api/projects/1`

**Method**: `DELETE`

**Expected Response (204 No Content)**:
```
(Empty body)
```

---

## ❌ Error Testing

### Test 1: Missing Required Field

**Request**:
```json
{
    "artistName": "The Composers",
    "createdBy": 1
}
```

**Expected Response (400 Bad Request)**:
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

---

### Test 2: Project Not Found

**URL**: `http://localhost:5000/api/projects/999`

**Method**: `GET`

**Expected Response (404 Not Found)**:
```json
{
    "error": "Project with id '999' not found."
}
```

---

### Test 3: Invalid Data (Too Long)

**Request**:
```json
{
    "title": "A".repeat(300),
    "artistName": "The Composers",
    "createdBy": 1
}
```

**Expected Response (400 Bad Request)**:
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

## ✅ Troubleshooting

If you still encounter errors:

1. **"Unknown column 'p.UpdatedAt'"**
   - ✅ Fixed: Run `dotnet ef database update`
   - Verify: Run `dotnet ef migrations list` to see all migrations

2. **Connection String Issues**
   - Check: `appsettings.json` for correct MySQL connection
   - Verify: MySQL server is running

3. **Port Issues**
   - Default: `http://localhost:5000`
   - Check: `Properties/launchSettings.json`

---

## 📋 Postman Quick Setup

1. Create a new collection: "StudioFlow Projects"
2. Add requests:
   - POST /api/projects (Create)
   - GET /api/projects (List)
   - GET /api/projects/1 (Get One)
   - PUT /api/projects/1 (Update)
   - DELETE /api/projects/1 (Delete)

3. Set base URL as environment variable: `{{base_url}}/api/projects`

4. Use `{{base_url}} = http://localhost:5000`

---

## 🎉 Ready to Test

All migrations have been applied. Your Projects API should now work correctly from Postman!

**Date**: March 16, 2026
**Status**: ✅ Database Updated & Ready

