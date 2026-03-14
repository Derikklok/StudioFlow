Here's the **complete project description** with all functionalities finalized and structured for implementation.

---

# **Music Production Project Management System (MP-PMS)**

## **1. Project Overview**

A web-based business workflow system for music production companies that tracks project lifecycle, sample usage, and rights clearance compliance. The system ensures no music release proceeds without proper licensing verification, bridging creative production with legal compliance.

**Core Value Proposition**: Prevent costly legal disputes by enforcing rights clearance before release, while giving producers visibility into their project's compliance status.

---

## **2. System Actors (Users)**

| Actor | Role Description | Permissions |
|-------|-----------------|-------------|
| **Producer** | Music creators who use samples/loops in their productions | Create projects, log sample usage, view own projects only, receive clearance notifications |
| **Administrator** | Legal/operations managers overseeing compliance | Full project visibility, approve/reject clearances, manage users, generate reports, system configuration |

---

## **3. Functional Requirements**

### **Module 1: User Management & Authentication**

| ID | Functionality | Description | Actor |
|----|--------------|-------------|-------|
| UM-001 | User Registration | Create account with email, password, name, role assignment (Producer or Admin) | Admin |
| UM-002 | User Login | Authenticate with email/password, receive JWT token for session | Both |
| UM-003 | Password Reset | Request password reset via email link | Both |
| UM-004 | View Profile | View and edit own profile information | Both |
| UM-005 | List Users | View all system users with filtering by role | Admin |
| UM-006 | Disable User | Deactivate user account (soft delete) | Admin |

---

### **Module 2: Project Management**

| ID | Functionality | Description | Actor |
|----|--------------|-------------|-------|
| PM-001 | Create Project | Create new project with: title, artist name, genre, description, deadline, target release date | Producer |
| PM-002 | Edit Project | Modify project details (only if not in "Released" status) | Producer (own), Admin (all) |
| PM-003 | Delete Project | Remove project and all associated sample logs (soft delete) | Producer (own), Admin (all) |
| PM-004 | View Project List | Dashboard view: all projects with status badges, filter by status/genre/date | Both |
| PM-005 | View Project Detail | Complete project view: details + sample usage list + compliance status | Both |
| PM-006 | Update Project Status | Progress through stages: Pre-production → Recording → Mixing → Mastering → Ready for Review → Released | Producer (forward only), Admin (any state) |
| PM-007 | Project Search | Search projects by title, artist name, or genre | Both |
| PM-008 | Project Analytics | View project count by status, upcoming deadlines, overdue projects | Admin |

**Project Status States:**
- `PRE_PRODUCTION` → `RECORDING` → `MIXING` → `MASTERING` → `READY_FOR_REVIEW` → `RELEASED`
- Additional terminal state: `ARCHIVED` (post-release)

---

### **Module 3: Sample Usage Logging**

| ID | Functionality | Description | Actor |
|----|--------------|-------------|-------|
| SL-001 | Log Sample Usage | Add sample to project with: sample name, source type (Internal Library / External Purchase / Free Download / Unknown / Original Creation), usage category (Drums / Bass / Melody / Vocals / FX / Other), file reference (path/URL/description), initial clearance status | Producer |
| SL-002 | Edit Sample Log | Modify sample details (except project association) | Producer (own project), Admin |
| SL-003 | Remove Sample Log | Delete sample entry from project | Producer (own project), Admin |
| SL-004 | View Sample List | See all samples for a project with clearance status indicators | Both |
| SL-005 | Bulk Import Samples | Import multiple samples via CSV/JSON for large projects | Admin |
| SL-006 | Sample Search | Search samples across all projects by name or rights holder | Admin |

**Sample Usage Categories:**
- `DRUMS` - Drum loops, percussion, beats
- `BASS` - Basslines, low-end elements
- `MELODY` - Instrumental melodies, chords, arpeggios
- `VOCALS` - Voice samples, chops, hooks
- `FX` - Sound effects, transitions, atmospheres
- `OTHER` - Uncategorized elements

---

### **Module 4: Rights Clearance Management**

| ID | Functionality | Description | Actor |
|----|--------------|-------------|-------|
| RC-001 | Submit for Clearance | Producer marks sample as "Needs Clearance" → triggers Admin notification | Producer |
| RC-002 | Review Clearance Request | Admin views pending clearance queue with sample details and project context | Admin |
| RC-003 | Approve Clearance | Admin approves: sets status to "Cleared", adds rights holder name, license type, expiration date, notes, upload contract reference | Admin |
| RC-004 | Reject Clearance | Admin rejects: sets status to "Rejected", provides rejection reason, suggests alternatives | Admin |
| RC-005 | Request More Info | Admin sends query back to Producer for additional details | Admin |
| RC-006 | Self-Declare Clearance | Producer marks sample as "Cleared" with evidence (receipt, license PDF) for Admin verification | Producer |
| RC-007 | View Clearance History | Audit trail: who changed status, when, and why | Both |
| RC-008 | Expiration Alerts | System flags samples approaching license expiration (30/60/90 days) | Admin |

**Clearance Status Workflow:**
```
NEEDS_CLEARANCE → [Admin Review] → CLEARED or REJECTED
     ↑___________________________________________|
     [Producer can resubmit if rejected]
     
PENDING_VERIFICATION → [Admin confirms] → CLEARED or back to NEEDS_CLEARANCE
[Producer self-declares with evidence]
```

**License Types:**
- `ROYALTY_FREE` - One-time purchase, unlimited use
- `CREATIVE_COMMONS` - Open license with attribution
- `COMMERCIAL_LICENSE` - Purchased license with terms
- `CUSTOM_AGREEMENT` - Direct negotiation with rights holder
- `ORIGINAL_CREATION` - Created by production team, no external rights
- `UNKNOWN` - Source/rights unclear (blocks release)

---

### **Module 5: Release Compliance & Gatekeeping**

| ID | Functionality | Description | Actor |
|----|--------------|-------------|-------|
| RG-001 | Pre-Release Compliance Check | System validates: all samples must be "CLEARED" or "ORIGINAL_CREATION" | System |
| RG-002 | Release Gate Block | If uncleared samples exist: block status change to RELEASED, display blocking samples list | System |
| RG-003 | Release Approval | If compliant: allow status change to RELEASED, log release timestamp and approver | Producer/Admin |
| RG-004 | Compliance Score Display | Visual indicator per project: Green (100% cleared) / Yellow (pending items) / Red (blocked) | Both |
| RG-005 | Generate Clearance Report | Export PDF/CSV: project details + all samples + clearance status + rights holders | Admin |
| RG-006 | Release Certificate | Generate signed document certifying compliance for legal records | Admin |

**Compliance Rules:**
- **GREEN**: All samples cleared or original, ready for release
- **YELLOW**: Samples pending clearance or verification, release blocked
- **RED**: Samples rejected or expired, requires immediate action

---

### **Module 6: Notification System**

| ID | Functionality | Description | Trigger |
|----|--------------|-------------|---------|
| NS-001 | Clearance Request Notification | Alert Admin when new sample needs clearance | Sample marked "Needs Clearance" |
| NS-002 | Clearance Approved Notification | Alert Producer when sample cleared | Admin approves clearance |
| NS-003 | Clearance Rejected Notification | Alert Producer with rejection reason | Admin rejects clearance |
| NS-004 | Information Request Notification | Alert Producer for additional documentation | Admin requests more info |
| NS-005 | Expiration Warning Notification | Alert Admin of approaching license expiration | 30/60/90 days before expiry |
| NS-006 | Release Blocked Notification | Alert Producer attempting release with uncleared samples | Release attempt fails compliance |
| NS-007 | Project Status Change Notification | Alert relevant parties of status updates | Manual status change |
| NS-008 | System Announcement | Broadcast messages from Admin to all users | Admin broadcast |

**Notification Delivery:**
- In-app notification bell with unread count
- Email notifications for critical events (clearance approval/rejection, release blocks)

---

### **Module 7: Reporting & Analytics**

| ID | Functionality | Description | Actor |
|----|--------------|-------------|-------|
| RA-001 | Project Status Dashboard | Visual charts: projects by status, upcoming deadlines, completion rate | Admin |
| RA-002 | Clearance Performance Metrics | Average clearance time, approval/rejection rates, backlog size | Admin |
| RA-003 | Sample Usage Analytics | Most used sample sources, popular usage categories, clearance success rate | Admin |
| RA-004 | Rights Holder Directory | List of all rights holders with contact info, active licenses count | Admin |
| RA-005 | Compliance Audit Report | Historical view: projects released with full compliance documentation | Admin |
| RA-006 | Producer Activity Report | Per-producer metrics: projects created, samples logged, clearance requests | Admin |

---

### **Module 8: System Administration**

| ID | Functionality | Description | Actor |
|----|--------------|-------------|-------|
| SA-001 | Genre Management | CRUD operations for genre list (Pop, Hip-Hop, Electronic, etc.) | Admin |
| SA-002 | Usage Category Management | CRUD for sample usage categories | Admin |
| SA-003 | License Type Configuration | Define available license types and terms | Admin |
| SA-004 | Notification Templates | Customize email/notification message templates | Admin |
| SA-005 | System Settings | Configure: clearance expiration warning days, max projects per producer, file reference format | Admin |
| SA-006 | Audit Log View | System-wide log of all create/update/delete operations | Admin |
| SA-007 | Data Export | Export full database or specific tables for backup | Admin |

---

## **4. Non-Functional Requirements**

| Category | Requirement |
|----------|-------------|
| **Performance** | Page load < 2 seconds; search results < 500ms; support 100 concurrent users |
| **Security** | Passwords hashed with bcrypt; JWT with expiration; HTTPS only; SQL injection prevention; XSS protection |
| **Availability** | 99% uptime; daily automated backups |
| **Usability** | Responsive design (desktop primary, tablet compatible); no audio file handling (text/forms only) |
| **Scalability** | Database design supports future expansion to 10,000 projects |
| **Maintainability** | REST API architecture; documented endpoints; modular codebase |

---

## **5. Business Rules (System Constraints)**

| Rule ID | Rule Description | Enforcement |
|---------|-----------------|-------------|
| BR-001 | A project cannot be marked "RELEASED" if any sample has status other than "CLEARED" or "ORIGINAL_CREATION" | Hard system block with error message |
| BR-002 | Producers can only view and edit their own projects | Database query filtering by created_by |
| BR-003 | Once a project is "RELEASED", no further sample additions or edits allowed | UI restrictions + backend validation |
| BR-004 | Sample clearance status changes must include timestamp and user attribution | Audit log entry required |
| BR-005 | Expired clearances automatically revert sample status to "NEEDS_CLEARANCE" | Scheduled background job |
| BR-006 | All notifications must be marked as read before certain actions (optional configuration) | UI reminder, non-blocking |

---

## **6. User Interface Requirements**

### **Producer Interface**
- **Dashboard**: My Projects list with compliance color-coding, quick "Add Sample" button, recent notifications
- **Project View**: Project details card + Sample Usage table (sortable/filterable) + Compliance Score badge + Release button (disabled if non-compliant)
- **Sample Log Form**: Modal or page with fields: Sample Name, Source Type (dropdown), Usage Category (dropdown), File Reference (text), Clearance Status (default "Needs Clearance"), Notes
- **Notifications Panel**: Dropdown bell icon with recent alerts, "Mark all read" option

### **Administrator Interface**
- **Dashboard**: System Overview (total projects, pending clearances, compliance rate), Pending Clearances queue, Recent Activity feed
- **Project Management**: All projects view with advanced filters (by producer, status, date range), bulk actions
- **Clearance Review Interface**: Side-by-side view: Sample details + Project context + Approval form (rights holder, license type, expiration, upload, notes)
- **Reports Center**: Date range selector + Report type dropdown + Generate/Export buttons
- **User Management**: User table with role badges, enable/disable toggle, reset password action

---

## **7. Data Retention & Archival**

| Data Type | Retention Policy |
|-----------|-----------------|
| Active Projects | Indefinite while in non-terminal status |
| Released Projects | Archive after 2 years (view-only, no edits) |
| Rejected Clearances | Keep for 1 year then purge |
| Audit Logs | 3 years then archive to cold storage |
| Notifications | 90 days then auto-delete (except email copies) |

---

## **8. Integration Points (Future Expansion)**

| Integration | Purpose | Priority |
|-------------|---------|----------|
| Email Service (SendGrid/AWS SES) | Notification delivery | Required |
| Cloud Storage (AWS S3) | Contract document storage | Phase 2 |
| External Royalty System | Sync clearance data | Future |
| DAW Plugin | Direct sample logging from production software | Future |

---

## **9. Success Criteria**

The system is considered complete when:

1. Producer can create project → log 10+ samples → submit for clearance → receive approval → mark project released (all within 5 minutes)
2. Admin can review 20 pending clearances → approve/reject → generate compliance report (all within 10 minutes)
3. System blocks release attempt on project with 1+ uncleared sample (100% enforcement rate)
4. All user actions create audit trail entries with timestamp and actor
5. Zero security vulnerabilities in authentication and authorization

---

## **10. Project Deliverables**

| Deliverable | Description |
|-------------|-------------|
| **Source Code** | Complete codebase with README setup instructions |
| **Database Schema** | SQL DDL scripts + ERD diagram |
| **API Documentation** | Swagger/OpenAPI spec with all endpoints |
| **User Manual** | Producer Guide + Admin Guide (PDF) |
| **Test Suite** | Unit tests + Integration tests (minimum 70% coverage) |
| **Demo Data** | Seed script with 5 sample projects, 20+ samples, 2 users |
| **Deployment Guide** | Server requirements + Configuration steps + Docker setup (optional) |

---
