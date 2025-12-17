# HealthStack Auth Service

The **Auth Service** handles user registration, login, JWT authentication, and roles for the HealthStack microservices ecosystem.

---

## Features

- User registration (email, password, first name, last name)  
- User login → returns JWT token  
- Get current authenticated user (`/me`)  
- Role-based authorization (`User` role by default)  
- Global exception handling with proper HTTP status codes  

---

## API Endpoints

| Endpoint | Method | Request DTO | Response DTO | Description |
|----------|--------|------------|--------------|-------------|
| `/api/user/register` | POST | `UserRegisterDto` | `RegisterResponseDto` | Register a new user and get JWT token |
| `/api/user/login` | POST | `UserLoginDto` | `LoginResponseDto` | Login existing user and get JWT token |
| `/api/user/me` | GET | None | `UserReadDto` | Get current authenticated user |

### Example: Register
```json
POST /api/user/register
{
  "email": "user@example.com",
  "password": "Password123!",
  "firstName": "Jane",
  "lastName": "Doe"
}
```

Response
```json
POST /api/user/register
{
  "token": "JWT_TOKEN_HERE",
  "user": {
    "id": "guid",
    "email": "user@example.com",
    "firstName": "Jane",
    "lastName": "Doe",
    "role": "User"
  }
}
```
### Example: Login
```json
POST /api/user/login
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

Response
```json
POST /api/user/register
{
  "token": "JWT_TOKEN_HERE",
  "user": {
    "id": "guid",
    "email": "user@example.com",
    "firstName": "Jane",
    "lastName": "Doe",
    "role": "User"
  }
}
```
### Example: Get Current User
```http
GET /api/user/me
Authorization: Bearer JWT_TOKEN_HERE
```

Response
```json
POST /api/user/register
{
  "id": "guid",
  "email": "user@example.com",
  "firstName": "Jane",
  "lastName": "Doe",
  "role": "User"
}
```

### Run Locally
```bash
# Run Auth service only
cd src/HealthStack.Auth.Api
dotnet run~
```

### Test
```bash
# Run all unit tests
dotnet test tests/HealthStack.Auth.UnitTests

# Run integration tests
dotnet test tests/HealthStack.Auth.IntegrationTests
```

## JWT Authentication

- A JWT token is issued upon **user registration** or **login**.  
- For endpoints that require authentication, include the token in the request header: ```Authorization: Bearer {token}```


- Token contains the following claims:
- `sub` → User ID
- `email` → User email
- `role` → User role

---

## Notes

- The **global exception handler** ensures consistent API error responses.  
- Passwords are securely hashed using **BCrypt**.  
- New users are assigned the **User** role by default.  
- **Swagger/OpenAPI documentation** is available in development mode via **Scalar**.

---

## Tech Stack

- .NET 10
- Entity Framework Core
- SQL Server
- JWT Authentication
- AutoMapper
- FluentValidation
- Scalar (API documentation)
- xUnit, Moq, FluentAssertions for testing
