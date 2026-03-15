# Authentication API - Quick Reference

## Endpoints

### 1. Register
```
POST /api/v1/auth/register
```

**Request:**
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "password123",
  "role": "PRODUCER"
}
```

**Success (201):** User created
**Error (400):** Duplicate email or validation failure
**Error (500):** Server error

---

### 2. Login
```
POST /api/v1/auth/login
```

**Request:**
```json
{
  "email": "john@example.com",
  "password": "password123"
}
```

**Success (200):** User authenticated
**Error (401):** Invalid credentials
**Error (400):** Validation failure
**Error (500):** Server error

---

## Error Responses

### Duplicate Email (400)
```json
{
  "error": "A user with email 'john@example.com' already exists. Please try logging in."
}
```

### Invalid Credentials (401)
```json
{
  "error": "Invalid email or password"
}
```

### Validation Error (400)
```json
{
  "email": ["Invalid email format"],
  "password": ["Password must be at least 6 characters"]
}
```

---

## File Locations

| Component | Location |
|-----------|----------|
| DTOs | `/DTOs/Auth/` |
| Exceptions | `/Exceptions/` |
| Repository | `/Repositories/AuthRepository.cs` |
| Service | `/Services/AuthService.cs` |
| Controller | `/Controllers/AuthController.cs` |

---

## Key Features

✅ Email uniqueness enforced  
✅ Separate from User management  
✅ Silent error handling (no console logs)  
✅ Proper HTTP status codes  
✅ Field-level validation  
✅ Active user check on login  

---

## Test Credentials

Use these to test (or create new ones):

```
Email: test@example.com
Password: password123
Role: PRODUCER
```

---

## Console Behavior

✅ NO console errors for:
- Duplicate email registration
- Invalid login credentials

✅ Console logs only for:
- Unexpected server errors
- Critical failures

---

## To Run

```bash
dotnet build
dotnet run
```

API will be available at: `https://localhost:7xxx`

---

## To Add JWT Later

Update `AuthService.cs`:
1. Add JWT token generation in `LoginAsync()`
2. Return token in `LoginResponse`
3. Add `[Authorize]` attributes to protected endpoints
4. Configure JWT middleware in `Program.cs`

For now, just validate credentials - no token needed.

