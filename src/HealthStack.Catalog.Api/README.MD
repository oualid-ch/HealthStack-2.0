# HealthStack Catalog Service

The **Catalog Service** is a microservice responsible for managing product and category information for the HealthStack eCommerce platform. It provides APIs to list, create, update, delete, and restore products and categories, supporting both public and admin access.

This service is built with **.NET 10**, **ASP.NET Core Web API**, **Entity Framework Core**, **AutoMapper**, and **FluentValidation**.

---

## Table of Contents
- [Features](#features)
- [Architecture](#architecture)
- [Endpoints](#endpoints)
- [Validation](#validation)
- [Testing](#testing)
- [Technologies](#technologies)

---

## Features

- CRUD operations for **Products** and **Categories**
- Role-based access control (`Admin` vs Public)
- DTO mapping using **AutoMapper**
- Input validation with **FluentValidation**
- Soft-delete and restore functionality
- Pagination and filtering support for products via **Gridify**
- Global exception handling
- Unit and integration testing
- Ready for Docker deployment

---

## Architecture

This service follows a **clean layered architecture**:
```
HealthStack.Catalog.Api
│
├─ Controllers # API endpoints
├─ Services # Business logic layer
├─ Models # Entity and domain models
├─ DTOs # Data transfer objects
├─ Validators # FluentValidation validators
├─ Mappings # AutoMapper profiles
├─ Data # EF Core DbContext and configuration
├─ Tests # Unit and integration tests
└─ Program.cs # App startup
```

**Database:** SQL Server (local or Docker)  
**Persistence:** EF Core (Code-First approach)

# Endpoints
## Categories

| Method | Endpoint                     | Role   | Description              |
| ------ | ---------------------------- | ------ | ------------------------ |
| GET    | `/api/category`              | Public | List all categories      |
| POST   | `/api/category`              | Admin  | Create a new category    |
| PUT    | `/api/category/{id}`         | Admin  | Update category          |
| DELETE | `/api/category/{id}`         | Admin  | Soft-delete category     |
| PUT    | `/api/category/{id}/restore` | Admin  | Restore deleted category |

## Products
| Method | Endpoint                    | Role   | Description                                 |
| ------ | --------------------------- | ------ | ------------------------------------------- |
| GET    | `/api/product`              | Public | List all products with pagination/filtering |
| GET    | `/api/product/{id}`         | Public | Get product by ID                           |
| GET    | `/api/product/sku/{sku}`    | Public | Get product by SKU                          |
| POST   | `/api/product`              | Admin  | Create a new product                        |
| PUT    | `/api/product/{id}`         | Admin  | Update product                              |
| DELETE | `/api/product/{id}`         | Admin  | Soft-delete product                         |
| PUT    | `/api/product/{id}/restore` | Admin  | Restore deleted product                     |

# Validation
- All input DTOs are validated using FluentValidation
- Invalid requests return 400 Bad Request with error details
- Validation occurs at the controller boundary

# Testing
Unit tests cover service layer logic
Integration tests cover API endpoints using in-memory database
Run tests:
```bash
dotnet test
```

# Technologies
- .NET 10 / ASP.NET Core Web API
- Entity Framework Core (Code-First)
- AutoMapper
- FluentValidation
- SQL Server
- Gridify (pagination & filtering)
- xUnit (unit & integration tests)
- Docker (optional for full stack)