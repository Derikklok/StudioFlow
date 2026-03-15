## Authentication API - Implementation Guide

### Overview
A complete authentication module has been implemented with the following endpoints:
- `POST /api/v1/auth/register` - Register a new user
- `POST /api/v1/auth/login` - Login with email and password

---

## Files Created/Modified

### New DTOs (DTOs/Auth/)
1. **RegisterRequest.cs** - Request DTO for user registration
   - Name (required, max 100 chars)
   - Email (required, must be valid email)
   - Password (required, min 6 chars)
   - Role (required, enum: PRODUCER, ADMIN)

2. **RegisterResponse.cs** - Response DTO for successful registration
   - Returns user details and success message

3. **LoginRequest.cs** - Request DTO for login
   - Email (required, must be valid email)
   - Password (required)

4. **LoginResponse.cs** - Response DTO for successful login
   - Returns user details and success message

### New Exceptions (Exceptions/)
1. **InvalidCredentialsException** - Thrown when email/password is invalid
   - Returns 401 Unauthorized

2. **UserAlreadyExistsException** - Thrown when email already exists during registration
   - Returns 400 Bad Request

### New Repository (Repositories/)
1. **IAuthRepository** - Interface for auth repository
2. **AuthRepository.cs** - Implementation
   - GetByEmailAsync() - Get user by email
   - CreateAsync() - Create new user with duplicate email handling

### New Service (Services/)
1. **IAuthService** - Interface for auth service (already existed)
2. **AuthService.cs** - Implementation
   - RegisterAsync() - Handle user registration
   - LoginAsync() - Handle user login

### New Controller (Controllers/)
1. **AuthController.cs** - API endpoints
   - POST /api/v1/auth/register
   - POST /api/v1/auth/login

### Updated Files
1. **Program.cs**
   - Added exception imports
   - Registered IAuthRepository & AuthRepository in DI
   - Registered IAuthService & AuthService in DI
   - Updated exception handlers for new exception types
   - Updated logging to suppress EF Core logs at Critical level

---

## API Usage Examples

### Register Endpoint
**POST** `/api/v1/auth/register`

Request:
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "password123",
  "role": "PRODUCER"
}
```

Success Response (201 Created):
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com",
  "role": "PRODUCER",
  "isActive": true,
  "createdAt": "2026-03-15T10:30:00Z",
  "message": "User registered successfully"
}
```

Error Response (400 Bad Request) - Duplicate Email:
```json
{
  "error": "A user with email 'john@example.com' already exists. Please try logging in."
}
```

Error Response (400 Bad Request) - Validation Error:
```json
{
  "name": ["Name is required"],
  "email": ["Invalid email format"],
  "password": ["Password must be at least 6 characters"]
}
```

### Login Endpoint
**POST** `/api/v1/auth/login`

Request:
```json
{
  "email": "john@example.com",
  "password": "password123"
}
```

Success Response (200 OK):
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com",
  "role": "PRODUCER",
  "isActive": true,
  "message": "Login successful"
}
```

Error Response (401 Unauthorized) - Invalid Credentials:
```json
{
  "error": "Invalid email or password"
}
```

---

## Error Handling

The system handles errors gracefully:

| Exception | HTTP Status | Description |
|-----------|-------------|-------------|
| UserAlreadyExistsException | 400 | Email already registered |
| InvalidCredentialsException | 401 | Wrong email or password |
| InvalidOperationException | 400 | Database duplicate error (caught as UserAlreadyExistsException) |
| Validation Errors | 400 | Model validation fails |
| Other Exceptions | 500 | Unexpected server errors |

All errors are logged silently (no console spam) unless they're unexpected.

---

## Current Limitations (As Requested)

1. ✅ NO JWT/Token Implementation
2. ✅ NO Password Hashing (plain text comparison)
3. ✅ Passwords stored as plain text in database

**TODO for Production:**
- Add BCrypt password hashing
- Implement JWT token generation
- Add token validation middleware
- Add refresh token mechanism
- Add password reset functionality
- Add email verification

---

## Architecture

```
DTOs/Auth/
  ├── RegisterRequest.cs
  ├── RegisterResponse.cs
  ├── LoginRequest.cs
  └── LoginResponse.cs

Exceptions/
  ├── InvalidCredentialsException.cs
  ├── UserAlreadyExistsException.cs
  └── DuplicateEmailException.cs

Repositories/
  ├── Interfaces/
  │   └── IAuthRepository.cs
  └── AuthRepository.cs

Services/
  ├── Interfaces/
  │   └── IAuthService.cs
  └── AuthService.cs

Controllers/
  └── AuthController.cs
```

---

## Testing Notes

- The auth module is completely separate from the Users module
- No JWT/token needed for testing - just pass credentials
- Passwords are currently plain text (use simple passwords like "password123")
- Database constraint ensures email uniqueness
- Active users only can login (IsActive must be true)

