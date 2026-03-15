# StudioFlow - Authentication API Test Collection

This document provides Postman requests to test the authentication endpoints.

## Base URL
```
https://localhost:7xxx/api/v1
```
Replace `7xxx` with your actual port number.

---

## 1. Register User

### Endpoint
```
POST /auth/register
```

### Request
```json
{
  "name": "Jane Smith",
  "email": "jane@example.com",
  "password": "password123",
  "role": "ADMIN"
}
```

### Expected Response (201 Created)
```json
{
  "id": 2,
  "name": "Jane Smith",
  "email": "jane@example.com",
  "role": "ADMIN",
  "isActive": true,
  "createdAt": "2026-03-15T12:00:00Z",
  "message": "User registered successfully"
}
```

---

## 2. Register User - Duplicate Email (Should Fail)

### Endpoint
```
POST /auth/register
```

### Request
```json
{
  "name": "Jane Smith",
  "email": "jane@example.com",
  "password": "password123",
  "role": "PRODUCER"
}
```

### Expected Response (400 Bad Request)
```json
{
  "error": "A user with email 'jane@example.com' already exists. Please try logging in."
}
```

---

## 3. Login Successful

### Endpoint
```
POST /auth/login
```

### Request
```json
{
  "email": "jane@example.com",
  "password": "password123"
}
```

### Expected Response (200 OK)
```json
{
  "id": 2,
  "name": "Jane Smith",
  "email": "jane@example.com",
  "role": "ADMIN",
  "isActive": true,
  "message": "Login successful"
}
```

---

## 4. Login - Invalid Password

### Endpoint
```
POST /auth/login
```

### Request
```json
{
  "email": "jane@example.com",
  "password": "wrongpassword"
}
```

### Expected Response (401 Unauthorized)
```json
{
  "error": "Invalid email or password"
}
```

---

## 5. Login - User Not Found

### Endpoint
```
POST /auth/login
```

### Request
```json
{
  "email": "nonexistent@example.com",
  "password": "password123"
}
```

### Expected Response (401 Unauthorized)
```json
{
  "error": "Invalid email or password"
}
```

---

## 6. Register - Validation Errors

### Endpoint
```
POST /auth/register
```

### Request (Missing required fields)
```json
{
  "name": "Test",
  "email": "invalid-email",
  "password": "123"
}
```

### Expected Response (400 Bad Request)
```json
{
  "name": [],
  "email": ["The Email field is not a valid e-mail address."],
  "password": ["The field Password must be a string or array type with a minimum length of '6'."],
  "role": ["The role field is required."]
}
```

---

## Testing Checklist

- [ ] Register new user successfully
- [ ] Cannot register with existing email
- [ ] Login with correct credentials
- [ ] Cannot login with wrong password
- [ ] Cannot login with non-existent email
- [ ] Validation errors for invalid input
- [ ] No console errors/logs when registering duplicate email
- [ ] No console errors/logs when login fails with invalid credentials
- [ ] Response includes user details on success
- [ ] Response includes proper error messages on failure
- [ ] HTTP status codes are correct (201, 200, 400, 401)

---

## Notes

- All passwords are currently in plain text (no hashing)
- Emails must be unique (database constraint enforced)
- Only active users (IsActive=true) can login
- No JWT/token returned - credentials are just validated
- All error responses are logged silently (no console spam)

