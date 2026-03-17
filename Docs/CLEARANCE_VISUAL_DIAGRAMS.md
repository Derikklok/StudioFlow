# 📊 CLEARANCE FEATURE - VISUAL DIAGRAMS & ARCHITECTURE

## 1. System Architecture Diagram

```
┌────────────────────────────────────────────────────────┐
│                   STUDIOFLOW SYSTEM                    │
├────────────────────────────────────────────────────────┤
│                                                        │
│  ┌──────────────────────────────────────────────┐     │
│  │           HTTP Requests (REST API)           │     │
│  └──────────┬───────────────────────────────────┘     │
│             │                                          │
│  ┌──────────▼───────────────────────────────────┐     │
│  │   Global Exception Handler Middleware        │     │
│  │  (Catches all exceptions → JSON Response)    │     │
│  └──────────┬───────────────────────────────────┘     │
│             │                                          │
│  ┌──────────▼───────────────────────────────────┐     │
│  │      ClearancesController (6 Endpoints)      │     │
│  │  POST /                  Create              │     │
│  │  GET  /                  Get All             │     │
│  │  GET  /{id}              Get One             │     │
│  │  PATCH /{id}             Update (Partial)    │     │
│  │  PUT  /{id}/approve      Approve             │     │
│  │  DELETE /{id}            Delete              │     │
│  └──────────┬───────────────────────────────────┘     │
│             │                                          │
│  ┌──────────▼───────────────────────────────────┐     │
│  │       ClearanceService (Business Logic)      │     │
│  │  • Validation                                │     │
│  │  • Exception Throwing                        │     │
│  │  • Data Mapping (Model ↔ DTO)                │     │
│  └──────────┬───────────────────────────────────┘     │
│             │                                          │
│  ┌──────────▼───────────────────────────────────┐     │
│  │   ClearanceRepository (Data Access Layer)    │     │
│  │  • AddAsync()                                │     │
│  │  • UpdateAsync()                             │     │
│  │  • DeleteAsync()                             │     │
│  │  • SaveChangesAsync()                        │     │
│  │  • GetByIdAsync()                            │     │
│  │  • GetAllAsync()                             │     │
│  └──────────┬───────────────────────────────────┘     │
│             │                                          │
│  ┌──────────▼───────────────────────────────────┐     │
│  │      Entity Framework Core + MySQL           │     │
│  │           Clearances Table                   │     │
│  │  ┌─────────────────────────────────────┐     │     │
│  │  │ Id (PK)                             │     │     │
│  │  │ SampleId (FK, UNIQUE)               │     │     │
│  │  │ RightsOwner (VARCHAR 200)           │     │     │
│  │  │ LicenseType (VARCHAR 255)           │     │     │
│  │  │ IsApproved (BOOLEAN)                │     │     │
│  │  │ ApprovedAt (DATETIME, NULLABLE)     │     │     │
│  │  │ Notes (VARCHAR 4000)                │     │     │
│  │  │ CreatedAt (DATETIME)                │     │     │
│  │  └─────────────────────────────────────┘     │     │
│  └─────────────────────────────────────────────┘     │
│                                                        │
└────────────────────────────────────────────────────────┘
```

---

## 2. Database Schema Relationships

```
┌─────────────────────┐
│      USERS          │
├─────────────────────┤
│ Id (PK)             │
│ Name                │
│ Email (UNIQUE)      │
│ Password            │
│ Role                │
│ IsActive            │
│ CreatedAt           │
└────────┬────────────┘
         │ 1:N
         │
┌────────▼────────────┐
│    PROJECTS         │
├─────────────────────┤
│ Id (PK)             │
│ Title               │ ◄─────────────────┐
│ Description         │                   │
│ Status              │                   │
│ UserId (FK)         │                   │
│ CreatedAt           │                   │
│ UpdatedAt           │                   │ CASCADE DELETE
└────────┬────────────┘                   │
         │ 1:N                            │
         │                                │
┌────────▼────────────┐                   │
│     SAMPLES         │                   │
├─────────────────────┤                   │
│ Id (PK)             │◄──────────────────┘
│ ProjectId (FK)  ────┘
│ Title               │
│ SourceArtist        │
│ SourceTrack         │
│ RightsHolder        │
│ Status              │
│ CreatedAt           │
│ UpdatedAt           │
└────────┬────────────┘
         │ 1:1 (CASCADE DELETE)
         │
┌────────▼──────────────┐
│   CLEARANCES          │
├───────────────────────┤
│ Id (PK)               │
│ SampleId (FK)  ◄──────┤
│ RightsOwner           │
│ LicenseType           │
│ IsApproved            │
│ ApprovedAt (NULL-OK)  │
│ Notes                 │
│ CreatedAt             │
└───────────────────────┘
```

---

## 3. API Endpoint Flow Diagram

```
REQUEST
   │
   ▼
┌──────────────────────────────────┐
│  HTTP Method & URL Validation    │
│  • Check method (POST/PATCH/etc) │
│  • Check route (/api/clearances) │
└───────────────┬──────────────────┘
                │
                ▼
        ┌───────────────┐
        │  CONTROLLER   │
        │  ClearancesC. │
        └───────┬───────┘
                │
        ┌───────▼────────────┐
        │   DTO Validation   │
        │  • Parse JSON      │
        │  • Check required  │
        │  • Type checking   │
        └───────┬────────────┘
                │
        ┌───────▼───────────┐
        │   SERVICE LAYER   │
        │ ClearanceService  │
        ├───────────────────┤
        │ Business Logic:   │
        │ • Validation      │
        │ • Processing      │
        │ • Exception throw │
        └───────┬───────────┘
                │
        ┌───────▼──────────┐
        │   REPOSITORY     │
        │ ClearanceRepo    │
        │ Database ops     │
        └───────┬──────────┘
                │
        ┌───────▼──────────┐
        │  ENTITY FRAMEWORK│
        │   & MySQL DB     │
        └───────┬──────────┘
                │
                ▼
        ┌──────────────────┐
        │  DATABASE        │
        │  Clearances      │
        │  Table           │
        └──────┬───────────┘
               │
               ▼
        (Data Operation)
               │
        ┌──────▼──────────┐
        │  Response DTO   │
        │  Mapping        │
        └──────┬──────────┘
               │
                ▼
        ┌──────────────────────┐
        │ JSON Serialization   │
        │ + HTTP Status Code   │
        └──────┬───────────────┘
               │
                ▼
    EXCEPTION MIDDLEWARE
    ┌────────────────────┐
    │ Catch exceptions?  │
    │ YES → Format err   │
    │ NO  → Return OK    │
    └──────┬─────────────┘
           │
            ▼
        RESPONSE
    (JSON + Status Code)
```

---

## 4. Request/Response Cycle Example

```
╔════════════════════════════════════════════════════════════╗
║                   CREATE CLEARANCE (POST)                  ║
╚════════════════════════════════════════════════════════════╝

REQUEST:
─────────────────────────────────────────────────────────────
POST /api/clearances HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "sampleId": 1,
  "rightsOwner": "Artist Name",
  "licenseType": "CC BY 4.0",
  "notes": "Free use"
}
─────────────────────────────────────────────────────────────

PROCESSING:
─────────────────────────────────────────────────────────────
1. Controller receives request
2. DTO validation (required fields)
3. Service creates Clearance entity
4. Repository adds to DbContext
5. SaveChangesAsync() commits
6. Response DTO mapping
7. JSON serialization
─────────────────────────────────────────────────────────────

RESPONSE:
─────────────────────────────────────────────────────────────
HTTP/1.1 201 Created
Content-Type: application/json
Location: /api/clearances/5

{
  "id": 5,
  "sampleId": 1,
  "rightsOwner": "Artist Name",
  "licenseType": "CC BY 4.0",
  "isApproved": false,
  "approvedAt": null,
  "notes": "Free use",
  "createdAt": "2026-03-17T14:30:00Z"
}
─────────────────────────────────────────────────────────────
```

---

## 5. Error Handling Flow

```
┌─────────────────────────────────────┐
│       Exception Occurs              │
└────────────┬────────────────────────┘
             │
    ┌────────▼────────────┐
    │  Exception Type?    │
    └──┬──┬──┬──┬─────────┘
       │  │  │  │
   ┌───┴┐ │  │  │
   │    │ │  │  │
┌──▼──┐ ▼─┴──┴──┴─────────────┬─────────────────┬──────────────┐
│ CF  │ Invalid        Project  │ Clearance      │   Generic
│NotF │ Oper           NotFound │ NotFound       │   Exception
└──┬──┘ ─ation           Ex    │ Exception      │
   │      Ex                    │                │
   │  (400)              (404)  │ (404)          │ (500)
   │                            │                │
   ▼                            ▼                ▼                ▼
┌──────────┐  ┌───────────────┐  ┌────────────────┐  ┌────────────┐
│ 404      │  │ 400           │  │ 404            │  │ 500        │
│ Not      │  │ Bad           │  │ Not            │  │ Internal   │
│ Found    │  │ Request       │  │ Found          │  │ Server     │
└──────────┘  └───────────────┘  └────────────────┘  └────────────┘
   │              │                  │                  │
   └──────┬───────┴──────────────────┴──────────────────┘
          │
          ▼
   ┌──────────────────────────────┐
   │  Format JSON Response        │
   │  {                           │
   │    "error": "message",       │
   │    "traceId": "id" (500 only)│
   │  }                           │
   └──────────────────────────────┘
          │
          ▼
      RETURN TO CLIENT
```

---

## 6. CRUD Operations Lifecycle

```
CREATE                  READ                    UPDATE
─────────────────────────────────────────────────────────────
POST /api/clearances    GET /api/clearances     PATCH /api/clearances/{id}
  │                       │                         │
  ├─ Create new        ├─ Get all records      ├─ Fetch existing
  ├─ Set defaults      ├─ Include relations    ├─ Partial update
  ├─ Save to DB        └─ Return array         ├─ Save changes
  └─ Return 201 ✓          │                    └─ Return 200 ✓
                           ▼
                        GET /api/clearances/{id}
                           │
                        ├─ Get by ID
                        ├─ Include relations
                        ├─ Validate exists
                        └─ Return 200 or 404


DELETE                   APPROVE
─────────────────────────────────────────────────────────────
DELETE /api/clearances/{id}   PUT /api/clearances/{id}/approve
  │                               │
  ├─ Fetch record              ├─ Fetch record
  ├─ Remove from DB            ├─ Check not approved
  ├─ Commit changes            ├─ Set IsApproved = true
  └─ Return 204 ✓              ├─ Set ApprovedAt = now
                                ├─ Save changes
                                └─ Return 200 ✓
```

---

## 7. Data Flow for Update (PATCH)

```
CLIENT SENDS:
────────────────────────────────────────────
PATCH /api/clearances/5
{
  "licenseType": "New License",
  "notes": "Updated notes"
}

                    ↓

CONTROLLER RECEIVES:
────────────────────────────────────────────
• Extracts ID: 5
• Parses DTO with partial fields

                    ↓

SERVICE LAYER:
────────────────────────────────────────────
UpdateClearanceAsync(5, UpdateClearanceDto)
  1. Get existing from DB
  2. Check if found (else throw exception)
  3. For each non-null field in DTO:
     - Update corresponding model field
  4. Save to database
  5. Return updated ClearanceResponse

                    ↓

DATABASE:
────────────────────────────────────────────
Before:
┌─────────────────────────────────────────┐
│ id: 5                                   │
│ licenseType: "Old License"              │
│ notes: "Old notes"                      │
│ ...other fields unchanged...            │
└─────────────────────────────────────────┘

After UPDATE SET:
┌─────────────────────────────────────────┐
│ id: 5                                   │
│ licenseType: "New License"  ◄─ CHANGED  │
│ notes: "Updated notes"      ◄─ CHANGED  │
│ ...other fields unchanged...            │
└─────────────────────────────────────────┘

                    ↓

RESPONSE SENT BACK:
────────────────────────────────────────────
HTTP/1.1 200 OK
{
  "id": 5,
  "sampleId": 1,
  "rightsOwner": "Artist Name",
  "licenseType": "New License",        ◄─ UPDATED
  "isApproved": false,
  "notes": "Updated notes",            ◄─ UPDATED
  "createdAt": "2026-03-17T14:00:00Z",
  ...
}
```

---

## 8. Error Scenario - Double Approval

```
FIRST APPROVAL:
────────────────────────────────────
PUT /api/clearances/5/approve
  │
  ├─ Fetch clearance #5
  ├─ Check IsApproved = false ✓
  ├─ Set IsApproved = true
  ├─ Set ApprovedAt = 2026-03-17T14:30:00Z
  ├─ Save to database
  │
  └─ Response: 200 OK
     { "message": "Clearance approved successfully." }

DB State:
[Clearance #5: IsApproved=TRUE, ApprovedAt=2026-03-17T14:30:00Z]


SECOND APPROVAL (SAME ID):
────────────────────────────────────
PUT /api/clearances/5/approve
  │
  ├─ Fetch clearance #5
  ├─ Check IsApproved = true ✗ FAILS
  ├─ Throw InvalidOperationException
  │  "Clearance is already approved."
  │
  └─ Exception caught by middleware

RESPONSE: 400 Bad Request
{
  "error": "Clearance is already approved."
}

(No console logging - clean error response)
```

---

## 9. Complete Approval Workflow

```
┌─────────────────────────────────────────────────────────┐
│        CLEARANCE APPROVAL WORKFLOW STATE DIAGRAM        │
└─────────────────────────────────────────────────────────┘

START
 │
 ├─ Create Clearance
 │  ↓
 │  [DRAFT]
 │   │ IsApproved = false
 │   │ ApprovedAt = null
 │   │
 │   ├─ [OPTIONAL] Update Details (PATCH)
 │   │  ↓
 │   │  [DRAFT - UPDATED]
 │   │
 │   └─ Approve Clearance (PUT /approve)
 │      ↓
 │      [APPROVED]
 │       │ IsApproved = true
 │       │ ApprovedAt = 2026-03-17T14:30:00Z
 │       │
 │       └─ Try Approve Again
 │          ↓
 │          [ERROR] 400 Bad Request
 │          "Clearance is already approved."
 │
 │
 └─ [APPROVED] → Delete (DELETE /{id})
                 ↓
               [DELETED]
                 (From database)
```

---

## 10. Repository & Service Pattern

```
                    Controller
                        │
                        │ Calls
                        ▼
                  IServiceInterface
                   (Contracts)
                        ▲
                        │ Implements
              ┌─────────┴─────────┐
              │                   │
        [Service Layer]  [Business Logic]
              │
              │ Calls
              ▼
         IRepositoryInterface
            (Contracts)
              ▲
              │ Implements
       ┌──────┴──────┐
       │             │
   [Repository] [Data Access]
       │
       │ Uses
       ▼
   DbContext
       │
       ▼
  MySQL Database
```

---

## 11. File Dependencies Map

```
Controllers/ClearancesController.cs
    ├─ Depends on: IClearanceService
    ├─ Uses: CreateClearanceDto
    ├─ Uses: UpdateClearanceDto
    └─ Returns: ClearanceResponse

Services/ClearanceService.cs
    ├─ Depends on: IClearanceRepository
    ├─ Uses: Clearance model
    ├─ Throws: ClearanceNotFoundException
    ├─ Throws: InvalidOperationException
    └─ Mapping: Clearance ↔ ClearanceResponse

Repositories/ClearanceRepository.cs
    ├─ Depends on: AppDbContext
    ├─ Uses: Clearance model
    ├─ Entity: DbSet<Clearance>
    └─ Operations: CRUD

Models/Clearance.cs
    ├─ Related to: Sample (Foreign Key)
    └─ Mapped by: Fluent API

DTOs/Clearances/*.cs
    ├─ CreateClearanceDto (POST input)
    ├─ UpdateClearanceDto (PATCH input)
    └─ ClearanceResponse (API output)

Program.cs
    ├─ Registers: IClearanceService
    ├─ Registers: IClearanceRepository
    ├─ Handler: ClearanceNotFoundException
    └─ Handler: InvalidOperationException
```

---

## 12. Response Status Code Map

```
┌────────┬──────────────────┬─────────────────────────────────┐
│ Status │ Method           │ Meaning                         │
├────────┼──────────────────┼─────────────────────────────────┤
│ 201    │ POST             │ Clearance created successfully  │
│ 200    │ GET /            │ Clearance(s) retrieved          │
│ 200    │ GET /{id}        │ Single clearance retrieved      │
│ 200    │ PATCH /{id}      │ Clearance updated successfully  │
│ 200    │ PUT /{id}/approve│ Clearance approved successfully │
│ 204    │ DELETE /{id}     │ Clearance deleted (no content)  │
│ 400    │ Any              │ Bad request / Validation error  │
│ 404    │ GET / PATCH / PUT│ Clearance not found             │
│ 500    │ Any              │ Unexpected server error         │
└────────┴──────────────────┴─────────────────────────────────┘
```

---

## Summary

This document provides visual representations of:
✅ System architecture
✅ Database relationships
✅ API flow
✅ Request/response cycle
✅ Error handling
✅ CRUD lifecycle
✅ Update flow
✅ Approval workflow
✅ Dependencies
✅ Status codes

All diagrams show how the Clearance feature integrates with StudioFlow's overall architecture and how data flows through the system.

