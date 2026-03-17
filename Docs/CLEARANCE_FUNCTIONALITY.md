# Clearance Module - Functionality & Testing Guide

## Overview

The Clearance module manages copyright and licensing clearances for audio samples. Each sample can have one clearance record that tracks the rights owner, license type, approval status, and licensing notes.

## Database Schema

### Clearance Table

| Column | Type | Description |
|--------|------|-------------|
| `Id` | int (PK) | Unique identifier |
| `SampleId` | int (FK) | Foreign key to Sample |
| `RightsOwner` | varchar(200) | Name of the rights/copyright holder |
| `LicenseType` | varchar(255) | Type of license (e.g., Creative Commons, Commercial, Public Domain) |
| `IsApproved` | boolean | Whether the clearance has been approved |
| `ApprovedAt` | datetime (nullable) | Timestamp when clearance was approved |
| `Notes` | varchar(4000) | Additional notes about the clearance |
| `CreatedAt` | datetime | Timestamp when clearance record was created |

### Relationship

- **One-to-One**: One Sample has exactly one Clearance (Clearance is optional for Sample)
- **Cascade Delete**: Deleting a Sample automatically deletes its associated Clearance

## API Endpoints

### 1. Create Clearance
**Endpoint:** `POST /api/clearances`

**Request Body:**
```json
{
  "sampleId": 1,
  "rightsOwner": "John Doe",
  "licenseType": "Creative Commons BY 4.0",
  "notes": "Contact John for commercial use"
}
```

**Success Response (201 Created):**
```json
{
  "id": 5,
  "sampleId": 1,
  "rightsOwner": "John Doe",
  "licenseType": "Creative Commons BY 4.0",
  "isApproved": false,
  "approvedAt": null,
  "notes": "Contact John for commercial use",
  "createdAt": "2026-03-17T10:30:45.123Z"
}
```

---

### 2. Get All Clearances
**Endpoint:** `GET /api/clearances`

**Success Response (200 OK):**
```json
[
  {
    "id": 1,
    "sampleId": 1,
    "rightsOwner": "Artist Name",
    "licenseType": "Commercial License",
    "isApproved": true,
    "approvedAt": "2026-03-16T14:20:30.000Z",
    "notes": "Full commercial rights",
    "createdAt": "2026-03-15T09:15:00.000Z"
  },
  {
    "id": 2,
    "sampleId": 2,
    "rightsOwner": "Jane Smith",
    "licenseType": "Non-Commercial",
    "isApproved": false,
    "approvedAt": null,
    "notes": "Awaiting approval",
    "createdAt": "2026-03-17T08:45:22.000Z"
  }
]
```

---

### 3. Get Clearance by ID
**Endpoint:** `GET /api/clearances/{id}`

**Path Parameters:**
- `id` (int): Clearance ID

**Success Response (200 OK):**
```json
{
  "id": 1,
  "sampleId": 1,
  "rightsOwner": "Artist Name",
  "licenseType": "Commercial License",
  "isApproved": true,
  "approvedAt": "2026-03-16T14:20:30.000Z",
  "notes": "Full commercial rights",
  "createdAt": "2026-03-15T09:15:00.000Z"
}
```

**Error Response (404 Not Found):**
```json
{
  "error": "Clearance with ID 999 not found."
}
```

---

### 4. Update Clearance (Partial Update)
**Endpoint:** `PATCH /api/clearances/{id}`

**Path Parameters:**
- `id` (int): Clearance ID

**Request Body (All fields optional):**
```json
{
  "rightsOwner": "Updated Name",
  "licenseType": "Updated License Type",
  "notes": "Updated notes",
  "isApproved": true
}
```

**Success Response (200 OK):**
```json
{
  "id": 1,
  "sampleId": 1,
  "rightsOwner": "Updated Name",
  "licenseType": "Updated License Type",
  "isApproved": true,
  "approvedAt": "2026-03-17T10:45:30.000Z",
  "notes": "Updated notes",
  "createdAt": "2026-03-15T09:15:00.000Z"
}
```

**Error Response (404 Not Found):**
```json
{
  "error": "Clearance with ID 999 not found."
}
```

---

### 5. Approve Clearance
**Endpoint:** `PUT /api/clearances/{id}/approve`

**Path Parameters:**
- `id` (int): Clearance ID

**Success Response (200 OK):**
```json
{
  "message": "Clearance approved successfully."
}
```

**Error Response (400 Bad Request):**
```json
{
  "error": "Clearance is already approved."
}
```

**Error Response (404 Not Found):**
```json
{
  "error": "Clearance with ID 999 not found."
}
```

---

### 6. Delete Clearance
**Endpoint:** `DELETE /api/clearances/{id}`

**Path Parameters:**
- `id` (int): Clearance ID

**Success Response (204 No Content):**
No response body

**Error Response (404 Not Found):**
```json
{
  "error": "Clearance with ID 999 not found."
}
```

---

## Testing with Postman

### Prerequisites
- Ensure the application is running on `http://localhost:5000` (or your configured port)
- At least one Sample exists in the database (ID = 1)

### Test Sequence

#### Test 1: Create a Clearance
1. **Method:** POST
2. **URL:** `http://localhost:5000/api/clearances`
3. **Headers:** 
   - Content-Type: application/json
4. **Body:**
```json
{
  "sampleId": 1,
  "rightsOwner": "Artist Name",
  "licenseType": "Creative Commons BY 4.0",
  "notes": "Non-commercial use only"
}
```
5. **Expected Status:** 201 Created
6. **Note:** Save the returned `id` for subsequent tests (e.g., 5)

---

#### Test 2: Get All Clearances
1. **Method:** GET
2. **URL:** `http://localhost:5000/api/clearances`
3. **Expected Status:** 200 OK
4. **Expected:** Array of all clearances

---

#### Test 3: Get Specific Clearance
1. **Method:** GET
2. **URL:** `http://localhost:5000/api/clearances/5`
3. **Expected Status:** 200 OK
4. **Expected:** Single clearance object with ID 5

---

#### Test 4: Partial Update (PATCH)
1. **Method:** PATCH
2. **URL:** `http://localhost:5000/api/clearances/5`
3. **Headers:** 
   - Content-Type: application/json
4. **Body:**
```json
{
  "licenseType": "Updated License",
  "notes": "Updated notes here"
}
```
5. **Expected Status:** 200 OK
6. **Expected:** Updated clearance object

---

#### Test 5: Approve Clearance
1. **Method:** PUT
2. **URL:** `http://localhost:5000/api/clearances/5/approve`
3. **Expected Status:** 200 OK
4. **Expected Response:**
```json
{
  "message": "Clearance approved successfully."
}
```

---

#### Test 6: Approve Already Approved (Should Fail)
1. **Method:** PUT
2. **URL:** `http://localhost:5000/api/clearances/5/approve`
3. **Expected Status:** 400 Bad Request
4. **Expected Response:**
```json
{
  "error": "Clearance is already approved."
}
```

---

#### Test 7: Delete Clearance
1. **Method:** DELETE
2. **URL:** `http://localhost:5000/api/clearances/5`
3. **Expected Status:** 204 No Content
4. **Expected:** No response body

---

#### Test 8: Get Deleted Clearance (Should Fail)
1. **Method:** GET
2. **URL:** `http://localhost:5000/api/clearances/5`
3. **Expected Status:** 404 Not Found
4. **Expected Response:**
```json
{
  "error": "Clearance with ID 5 not found."
}
```

---

## Error Handling

### Exception Types

| Exception | HTTP Status | Message |
|-----------|------------|---------|
| ClearanceNotFoundException | 404 | "Clearance with ID {id} not found." |
| InvalidOperationException | 400 | "Clearance is already approved." |
| Any other exception | 500 | "An unexpected error occurred. Please try again later." |

### Global Exception Middleware

The application includes a global exception handler middleware that:
- Catches all exceptions without logging verbose stack traces
- Returns clean, user-friendly error messages
- Sets appropriate HTTP status codes
- Sends error information as JSON

---

## Implementation Details

### Service Layer (`ClearanceService`)
- **CreateClearanceAsync:** Creates new clearance record
- **GetAllAsync:** Retrieves all clearances with related Sample data
- **GetByIdAsync:** Gets specific clearance by ID
- **UpdateClearanceAsync:** Partially updates clearance (PATCH)
- **ApproveClearanceAsync:** Marks clearance as approved with timestamp
- **DeleteClearanceAsync:** Removes clearance record

### Repository Pattern
- `IClearanceRepository` defines data access contracts
- `ClearanceRepository` implements database operations
- Separation of concerns between service and data layers

### DTOs (Data Transfer Objects)
- `CreateClearanceDto`: Request model for creating clearance
- `UpdateClearanceDto`: Request model for updating clearance
- `ClearanceResponse`: Response model for API responses

---

## Business Logic Rules

1. **Sample Requirement:** Clearance must reference an existing Sample
2. **One Clearance per Sample:** Database enforces one-to-one relationship
3. **Approval State:** Cannot be re-approved once already approved
4. **Cascade Delete:** Deleting a Sample automatically deletes its Clearance
5. **Approval Timestamp:** `ApprovedAt` is automatically set when clearance is approved

---

## Future Enhancements

- [ ] Add clearance expiration dates
- [ ] Support for multiple clearances per sample (revision)
- [ ] Clearance templates for common license types
- [ ] Export clearance reports
- [ ] Audit trail for approval history
- [ ] Email notifications on approval/rejection

