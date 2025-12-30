# HealthStack 2.0

**HealthStack** is a microservices-based eCommerce platform focusing on health products.  
This is a Phase 1 MVP showcasing backend skills with .NET 10, EF Core, JWT authentication, and integration/unit testing.

---

## Architecture Overview

| Service | Description |
|---------|------------|
| [Auth Service](./src/HealthStack.Auth.Api/README.md) | Handles user registration, login, JWT authentication, and roles |
| [Catalog Service](./src/HealthStack.Catalog.Api/README.MD) | Manages products |
| Order Service | Handles order lifecycle |

**JWT Authentication Flow:**  
1. User registers or logs in via Auth service  
2. Auth service returns a JWT token  
3. Other services use JWT to identify user  

**Diagrams:**
*Not created yet*

## Getting Started

### Requirements
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (or use Docker)
- Docker & Docker Compose (optional for multi-service setup)