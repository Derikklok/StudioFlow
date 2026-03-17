# StudioFlow Complete System Workflow - Step by Step

## System Overview

StudioFlow is an audio sample and project management system designed to handle the complete lifecycle of audio projects, from creation through sample management to licensing clearance. The system provides APIs for user authentication, project management, sample tracking, and clearance handling.

---

## Complete Workflow Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                      STUDIOFLOW SYSTEM                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────┐   ┌──────────────┐   ┌──────────────┐        │
│  │     AUTH     │   │    USERS     │   │  PROJECTS    │        │
│  │  (Login)     │→→→│ (Profiles)   │   │ (Container)  │        │
│  └──────────────┘   └──────────────┘   └──────┬───────┘        │
│                                               │                │
│                                         ┌─────▼───────┐        │
│                                         │   SAMPLES   │        │
│                                         │(Audio Files)│        │
│                                         └─────┬───────┘        │
│                                               │                │
│                                         ┌─────▼─────────────┐  │
│                                         │   CLEARANCES      │  │
│                                         │(Rights & License) │  │
│                                         └───────────────────┘  │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## Step-by-Step Workflow

### PHASE 1: USER MANAGEMENT & AUTHENTICATION

#### Step 1.1: Register a New User
**Purpose:** Create user account for the system

**Endpoint:** `POST /api/auth/register`

**Request:**
```json
{
  "name": "John Producer",
  "email": "john@example.com",
  "password": "securePassword123",
  "role": "Producer"
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "name": "John Producer",
  "email": "john@example.com",
  "role": "Producer",
  "isActive": true,
  "createdAt": "2026-03-17T10:00:00Z"
}
```

**Error Handling:**
- 400 Bad Request: Duplicate email
- 400 Bad Request: Missing required fields

---

#### Step 1.2: Login User
**Purpose:** Authenticate user and verify credentials

**Endpoint:** `POST /api/auth/login`

**Request:**
```json
{
  "email": "john@example.com",
  "password": "securePassword123"
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "name": "John Producer",
  "email": "john@example.com",
  "role": "Producer",
  "isActive": true,
  "message": "Login successful"
}
```

**Error Handling:**
- 401 Unauthorized: Invalid credentials
- 401 Unauthorized: User not found

---

#### Step 1.3: Get User Profile
**Purpose:** Retrieve current user details

**Endpoint:** `GET /api/users/{userId}`

**Response (200 OK):**
```json
{
  "id": 1,
  "name": "John Producer",
  "email": "john@example.com",
  "role": "Producer",
  "isActive": true,
  "createdAt": "2026-03-17T10:00:00Z"
}
```

---

### PHASE 2: PROJECT MANAGEMENT

#### Step 2.1: Create a New Project
**Purpose:** Create a container for audio samples

**Endpoint:** `POST /api/projects`

**Request:**
```json
{
  "title": "Summer Album 2026",
  "description": "Compilation of summer tracks",
  "userId": 1
}
```

**Response (201 Created):**
```json
{
  "id": 10,
  "title": "Summer Album 2026",
  "description": "Compilation of summer tracks",
  "status": "DRAFT",
  "userId": 1,
  "createdAt": "2026-03-17T11:00:00Z",
  "updatedAt": "2026-03-17T11:00:00Z"
}
```

**Status Values:** DRAFT, IN_PROGRESS, COMPLETED, ARCHIVED

---

#### Step 2.2: Get All Projects
**Purpose:** Retrieve all projects in the system

**Endpoint:** `GET /api/projects`

**Response (200 OK):**
```json
[
  {
    "id": 10,
    "title": "Summer Album 2026",
    "description": "Compilation of summer tracks",
    "status": "DRAFT",
    "userId": 1,
    "createdAt": "2026-03-17T11:00:00Z",
    "updatedAt": "2026-03-17T11:00:00Z"
  },
  {
    "id": 11,
    "title": "Winter Collection",
    "description": "Ambient tracks",
    "status": "IN_PROGRESS",
    "userId": 1,
    "createdAt": "2026-03-16T09:30:00Z",
    "updatedAt": "2026-03-17T10:15:00Z"
  }
]
```

---

#### Step 2.3: Get Project by ID
**Purpose:** Retrieve specific project details

**Endpoint:** `GET /api/projects/{projectId}`

**Response (200 OK):**
```json
{
  "id": 10,
  "title": "Summer Album 2026",
  "description": "Compilation of summer tracks",
  "status": "DRAFT",
  "userId": 1,
  "createdAt": "2026-03-17T11:00:00Z",
  "updatedAt": "2026-03-17T11:00:00Z"
}
```

---

#### Step 2.4: Update Project (Partial)
**Purpose:** Modify project details

**Endpoint:** `PATCH /api/projects/{projectId}`

**Request (All fields optional):**
```json
{
  "title": "Summer Album 2026 - Revised",
  "description": "Updated description",
  "status": "IN_PROGRESS"
}
```

**Response (200 OK):**
```json
{
  "id": 10,
  "title": "Summer Album 2026 - Revised",
  "description": "Updated description",
  "status": "IN_PROGRESS",
  "userId": 1,
  "createdAt": "2026-03-17T11:00:00Z",
  "updatedAt": "2026-03-17T12:30:00Z"
}
```

---

#### Step 2.5: Delete Project
**Purpose:** Remove project and associated samples/clearances

**Endpoint:** `DELETE /api/projects/{projectId}`

**Response (204 No Content)**

**Cascade:** Deletes all samples in the project, which cascade-deletes all clearances

---

### PHASE 3: SAMPLE MANAGEMENT

#### Step 3.1: Add Sample to Project
**Purpose:** Upload/register audio sample

**Endpoint:** `POST /api/samples`

**Request:**
```json
{
  "projectId": 10,
  "title": "Beautiful Melody",
  "sourceArtist": "Original Artist",
  "sourceTrack": "Original Track Name",
  "rightsHolder": "Rights Holder Name"
}
```

**Response (201 Created):**
```json
{
  "id": 100,
  "projectId": 10,
  "title": "Beautiful Melody",
  "sourceArtist": "Original Artist",
  "sourceTrack": "Original Track Name",
  "rightsHolder": "Rights Holder Name",
  "status": "DRAFT",
  "createdAt": "2026-03-17T12:00:00Z",
  "updatedAt": "2026-03-17T12:00:00Z"
}
```

**Status Values:** DRAFT, UNDER_REVIEW, APPROVED, REJECTED, ARCHIVED

---

#### Step 3.2: Get All Samples
**Purpose:** List all samples

**Endpoint:** `GET /api/samples`

**Response (200 OK):**
```json
[
  {
    "id": 100,
    "projectId": 10,
    "title": "Beautiful Melody",
    "sourceArtist": "Original Artist",
    "sourceTrack": "Original Track Name",
    "rightsHolder": "Rights Holder Name",
    "status": "DRAFT",
    "createdAt": "2026-03-17T12:00:00Z",
    "updatedAt": "2026-03-17T12:00:00Z"
  }
]
```

---

#### Step 3.3: Get Sample by ID
**Purpose:** Retrieve specific sample details

**Endpoint:** `GET /api/samples/{sampleId}`

**Response (200 OK):**
```json
{
  "id": 100,
  "projectId": 10,
  "title": "Beautiful Melody",
  "sourceArtist": "Original Artist",
  "sourceTrack": "Original Track Name",
  "rightsHolder": "Rights Holder Name",
  "status": "DRAFT",
  "createdAt": "2026-03-17T12:00:00Z",
  "updatedAt": "2026-03-17T12:00:00Z"
}
```

---

#### Step 3.4: Update Sample (Partial)
**Purpose:** Modify sample information

**Endpoint:** `PATCH /api/samples/{sampleId}`

**Request (All fields optional):**
```json
{
  "title": "Beautiful Melody - Remix",
  "status": "UNDER_REVIEW"
}
```

**Response (200 OK):**
```json
{
  "id": 100,
  "projectId": 10,
  "title": "Beautiful Melody - Remix",
  "sourceArtist": "Original Artist",
  "sourceTrack": "Original Track Name",
  "rightsHolder": "Rights Holder Name",
  "status": "UNDER_REVIEW",
  "createdAt": "2026-03-17T12:00:00Z",
  "updatedAt": "2026-03-17T13:45:00Z"
}
```

---

#### Step 3.5: Delete Sample
**Purpose:** Remove sample and its clearance

**Endpoint:** `DELETE /api/samples/{sampleId}`

**Response (204 No Content)**

**Cascade:** Automatically deletes associated clearance record

---

### PHASE 4: CLEARANCE & LICENSING

#### Step 4.1: Create Clearance for Sample
**Purpose:** Register licensing/rights information

**Endpoint:** `POST /api/clearances`

**Request:**
```json
{
  "sampleId": 100,
  "rightsOwner": "Music Rights Ltd",
  "licenseType": "Creative Commons BY-SA 4.0",
  "notes": "Must credit original artist in any publication"
}
```

**Response (201 Created):**
```json
{
  "id": 500,
  "sampleId": 100,
  "rightsOwner": "Music Rights Ltd",
  "licenseType": "Creative Commons BY-SA 4.0",
  "isApproved": false,
  "approvedAt": null,
  "notes": "Must credit original artist in any publication",
  "createdAt": "2026-03-17T14:00:00Z"
}
```

---

#### Step 4.2: Review Clearance Details
**Purpose:** Examine clearance information before approval

**Endpoint:** `GET /api/clearances/{clearanceId}`

**Response (200 OK):**
```json
{
  "id": 500,
  "sampleId": 100,
  "rightsOwner": "Music Rights Ltd",
  "licenseType": "Creative Commons BY-SA 4.0",
  "isApproved": false,
  "approvedAt": null,
  "notes": "Must credit original artist in any publication",
  "createdAt": "2026-03-17T14:00:00Z"
}
```

---

#### Step 4.3: Update Clearance Information (Partial)
**Purpose:** Modify clearance details before approval

**Endpoint:** `PATCH /api/clearances/{clearanceId}`

**Request:**
```json
{
  "licenseType": "Commercial License",
  "notes": "Full commercial rights granted"
}
```

**Response (200 OK):**
```json
{
  "id": 500,
  "sampleId": 100,
  "rightsOwner": "Music Rights Ltd",
  "licenseType": "Commercial License",
  "isApproved": false,
  "approvedAt": null,
  "notes": "Full commercial rights granted",
  "createdAt": "2026-03-17T14:00:00Z"
}
```

---

#### Step 4.4: Approve Clearance
**Purpose:** Finalize and approve the clearance

**Endpoint:** `PUT /api/clearances/{clearanceId}/approve`

**Response (200 OK):**
```json
{
  "message": "Clearance approved successfully."
}
```

**After Approval, Get Updated Clearance:**

**Endpoint:** `GET /api/clearances/{clearanceId}`

**Response:**
```json
{
  "id": 500,
  "sampleId": 100,
  "rightsOwner": "Music Rights Ltd",
  "licenseType": "Commercial License",
  "isApproved": true,
  "approvedAt": "2026-03-17T14:15:00Z",
  "notes": "Full commercial rights granted",
  "createdAt": "2026-03-17T14:00:00Z"
}
```

---

#### Step 4.5: Get All Clearances
**Purpose:** View all clearance records

**Endpoint:** `GET /api/clearances`

**Response (200 OK):**
```json
[
  {
    "id": 500,
    "sampleId": 100,
    "rightsOwner": "Music Rights Ltd",
    "licenseType": "Commercial License",
    "isApproved": true,
    "approvedAt": "2026-03-17T14:15:00Z",
    "notes": "Full commercial rights granted",
    "createdAt": "2026-03-17T14:00:00Z"
  }
]
```

---

#### Step 4.6: Delete Clearance
**Purpose:** Remove clearance record

**Endpoint:** `DELETE /api/clearances/{clearanceId}`

**Response (204 No Content)**

---

## Complete End-to-End Test Scenario

### Scenario: Create a Music Project with Samples and Clearances

#### Executive Summary
A music producer creates a new project "Summer Vibes", adds three audio samples, and obtains clearances for each sample.

---

### Test Sequence

#### 1. User Registration
```http
POST http://localhost:5000/api/auth/register
Content-Type: application/json

{
  "name": "Sarah Music Producer",
  "email": "sarah@musicstudio.com",
  "password": "securePass123",
  "role": "Producer"
}
```

**Expected:** Status 201, User ID = 1

---

#### 2. User Login
```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "sarah@musicstudio.com",
  "password": "securePass123"
}
```

**Expected:** Status 200, Authentication successful

---

#### 3. Create Project
```http
POST http://localhost:5000/api/projects
Content-Type: application/json

{
  "title": "Summer Vibes 2026",
  "description": "Collection of upbeat summer tracks",
  "userId": 1
}
```

**Expected:** Status 201, Project ID = 10

---

#### 4. Create First Sample
```http
POST http://localhost:5000/api/samples
Content-Type: application/json

{
  "projectId": 10,
  "title": "Beach Party Mix",
  "sourceArtist": "DJ Cool",
  "sourceTrack": "Original Beach Beats",
  "rightsHolder": "DJ Cool Productions"
}
```

**Expected:** Status 201, Sample ID = 100

---

#### 5. Create Clearance for First Sample
```http
POST http://localhost:5000/api/clearances
Content-Type: application/json

{
  "sampleId": 100,
  "rightsOwner": "DJ Cool Productions",
  "licenseType": "Creative Commons BY 4.0",
  "notes": "Attribution required"
}
```

**Expected:** Status 201, Clearance ID = 500

---

#### 6. Update Sample Status
```http
PATCH http://localhost:5000/api/samples/100
Content-Type: application/json

{
  "status": "APPROVED"
}
```

**Expected:** Status 200

---

#### 7. Approve Clearance
```http
PUT http://localhost:5000/api/clearances/500/approve
```

**Expected:** Status 200, "Clearance approved successfully."

---

#### 8. Create Second Sample
```http
POST http://localhost:5000/api/samples
Content-Type: application/json

{
  "projectId": 10,
  "title": "Tropical Vibes",
  "sourceArtist": "Island Sounds",
  "sourceTrack": "Caribbean Dreams",
  "rightsHolder": "Island Records"
}
```

**Expected:** Status 201, Sample ID = 101

---

#### 9. Create Clearance for Second Sample
```http
POST http://localhost:5000/api/clearances
Content-Type: application/json

{
  "sampleId": 101,
  "rightsOwner": "Island Records",
  "licenseType": "Commercial License",
  "notes": "Full rights for commercial use"
}
```

**Expected:** Status 201, Clearance ID = 501

---

#### 10. Approve Second Clearance
```http
PUT http://localhost:5000/api/clearances/501/approve
```

**Expected:** Status 200

---

#### 11. Get All Clearances to Verify
```http
GET http://localhost:5000/api/clearances
```

**Expected:** Status 200, Array with 2 approved clearances

---

#### 12. Update Project Status
```http
PATCH http://localhost:5000/api/projects/10
Content-Type: application/json

{
  "status": "COMPLETED"
}
```

**Expected:** Status 200, Project marked as COMPLETED

---

#### 13. Get Project Details
```http
GET http://localhost:5000/api/projects/10
```

**Expected:** Status 200, Project with status = COMPLETED

---

## Data Flow Diagram

```
USER
  │
  ├─→ [REGISTER] → Create User Record
  │
  ├─→ [LOGIN] → Verify Credentials
  │
  └─→ [CREATE PROJECT] 
      │
      ├─→ Project Created (DRAFT status)
      │
      └─→ [ADD SAMPLES]
          │
          ├─→ Sample 1 Created
          │   └─→ [CREATE CLEARANCE]
          │       └─→ Clearance 1 (NOT APPROVED)
          │           └─→ [APPROVE CLEARANCE]
          │               └─→ Clearance 1 (APPROVED with timestamp)
          │
          ├─→ Sample 2 Created
          │   └─→ [CREATE CLEARANCE]
          │       └─→ Clearance 2 (NOT APPROVED)
          │           └─→ [APPROVE CLEARANCE]
          │               └─→ Clearance 2 (APPROVED with timestamp)
          │
          └─→ Sample 3 Created
              └─→ [CREATE CLEARANCE]
                  └─→ Clearance 3 (NOT APPROVED)
                      └─→ [APPROVE CLEARANCE]
                          └─→ Clearance 3 (APPROVED with timestamp)

[UPDATE PROJECT STATUS] → Project (COMPLETED)
```

---

## Error Scenarios & Handling

### Scenario 1: Duplicate Email Registration
```http
POST http://localhost:5000/api/auth/register
```
**Response (400 Bad Request):**
```json
{
  "error": "Email already exists"
}
```

---

### Scenario 2: Invalid Login Credentials
```http
POST http://localhost:5000/api/auth/login
```
**Response (401 Unauthorized):**
```json
{
  "error": "Invalid credentials"
}
```

---

### Scenario 3: Approve Already Approved Clearance
```http
PUT http://localhost:5000/api/clearances/500/approve
```
(After already approving once)

**Response (400 Bad Request):**
```json
{
  "error": "Clearance is already approved."
}
```

---

### Scenario 4: Access Non-existent Resource
```http
GET http://localhost:5000/api/clearances/9999
```

**Response (404 Not Found):**
```json
{
  "error": "Clearance with ID 9999 not found."
}
```

---

## Database Schema Relationships

```
┌─────────────┐
│    Users    │
├─────────────┤
│ ID (PK)     │
│ Name        │
│ Email (UQ)  │
│ Password    │
│ Role        │
│ IsActive    │
│ CreatedAt   │
└──────┬──────┘
       │ 1:N
       │
┌──────▼──────┐
│  Projects   │
├─────────────┤
│ ID (PK)     │
│ Title       │
│ Description │
│ Status      │
│ UserId (FK) │
│ CreatedAt   │
│ UpdatedAt   │
└──────┬──────┘
       │ 1:N (CASCADE DELETE)
       │
┌──────▼──────┐
│  Samples    │
├─────────────┤
│ ID (PK)     │
│ ProjectId   │
│ Title       │
│ SourceArtis │
│ SourceTrack │
│ RightsHolder│
│ Status      │
│ CreatedAt   │
│ UpdatedAt   │
└──────┬──────┘
       │ 1:1 (CASCADE DELETE)
       │
┌──────▼──────────┐
│  Clearances     │
├─────────────────┤
│ ID (PK)         │
│ SampleId (FK)   │
│ RightsOwner     │
│ LicenseType     │
│ IsApproved      │
│ ApprovedAt      │
│ Notes           │
│ CreatedAt       │
└─────────────────┘
```

---

## System Features Summary

| Feature | Endpoint | Method | Status | Purpose |
|---------|----------|--------|--------|---------|
| Register | /api/auth/register | POST | ✓ | Create new user |
| Login | /api/auth/login | POST | ✓ | Authenticate user |
| Get User | /api/users/{id} | GET | ✓ | Retrieve user profile |
| Create Project | /api/projects | POST | ✓ | Create project |
| Get Projects | /api/projects | GET | ✓ | List all projects |
| Get Project | /api/projects/{id} | GET | ✓ | Get project details |
| Update Project | /api/projects/{id} | PATCH | ✓ | Modify project |
| Delete Project | /api/projects/{id} | DELETE | ✓ | Remove project |
| Create Sample | /api/samples | POST | ✓ | Add sample to project |
| Get Samples | /api/samples | GET | ✓ | List all samples |
| Get Sample | /api/samples/{id} | GET | ✓ | Get sample details |
| Update Sample | /api/samples/{id} | PATCH | ✓ | Modify sample |
| Delete Sample | /api/samples/{id} | DELETE | ✓ | Remove sample |
| Create Clearance | /api/clearances | POST | ✓ | Register clearance |
| Get Clearances | /api/clearances | GET | ✓ | List all clearances |
| Get Clearance | /api/clearances/{id} | GET | ✓ | Get clearance details |
| Update Clearance | /api/clearances/{id} | PATCH | ✓ | Modify clearance |
| Approve Clearance | /api/clearances/{id}/approve | PUT | ✓ | Approve clearance |
| Delete Clearance | /api/clearances/{id} | DELETE | ✓ | Remove clearance |

---

## Key Implementation Notes

1. **Cascade Deletes:** Deleting a Project deletes all Samples, which cascade-deletes all Clearances
2. **One-to-One Relationship:** Each Sample has at most one Clearance
3. **Global Exception Handling:** All exceptions are caught and formatted as JSON responses
4. **Validation:** Email uniqueness, required fields, and proper status enums are enforced
5. **Timestamps:** All entities track CreatedAt and UpdatedAt (where applicable)
6. **Statuses:** Projects and Samples use enum statuses for type safety

---

## Performance Considerations

- Database indexes on foreign keys for efficient queries
- Eager loading of related entities (Sample.Project, Clearance.Sample)
- Async/await for non-blocking I/O
- Connection pooling for database connections

---

## Security Considerations

- Email uniqueness prevents duplicate accounts
- Password validation at registration
- Role-based system for future authorization
- IsActive flag for user status management
- Global exception handler prevents information leakage

---

## Future Roadmap

- [ ] JWT token-based authentication
- [ ] Role-based access control (RBAC)
- [ ] File upload service for audio files
- [ ] Clearance expiration and renewal
- [ ] Audit logging for all operations
- [ ] Export functionality for reports
- [ ] Batch operations for bulk updates
- [ ] Search and filtering capabilities

