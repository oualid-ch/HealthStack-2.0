# HealthStack Ordering Service

The **Ordering Service** is a microservice responsible for managing customer orders in the HealthStack eCommerce platform. It allows authenticated users to create orders and retrieve their order history.
During order creation, the service synchronously communicates with the **Catalog Service** to fetch product details (name and price) and stores them as a snapshot in the order.

This service is built with **.NET 10**, **ASP.NET Core Web API**, **Entity Framework Core**, **AutoMapper**, and **FluentValidation**, and demonstrates **synchronous inter-service communication** in a microservices architecture.

---

## Table of Contents

* [Features](#features)
* [Architecture](#architecture)
* [Service Communication](#service-communication)
* [Endpoints](#endpoints)
* [Validation](#validation)
* [Technologies](#technologies)

---

## Features

* Create and retrieve **Orders**
* Orders scoped to the authenticated user
* Snapshotting of product data (name & price) at order time
* Synchronous HTTP communication with Catalog Service
* Role-based access control (`User`)
* DTO mapping using **AutoMapper**
* Input validation with **FluentValidation**
* Pagination and filtering support via **Gridify**
* Global exception handling
* Ready for Docker deployment

---

## Architecture

This service follows a **clean layered architecture**:

```
HealthStack.Order.Api
│
├─ Controllers # API endpoints
├─ Services # Business logic layer
├─ Clients # HTTP clients (Catalog integration)
├─ Models # Entity and domain models
├─ DTOs # Data transfer objects
├─ Validators # FluentValidation validators
├─ Mappings # AutoMapper profiles
├─ Auth # Current user abstraction (JWT claims)
├─ Data # EF Core DbContext and configuration
└─ Program.cs # App startup
```

**Database:** SQL Server (local or Docker)
**Persistence:** EF Core (Code-First approach)

---

## Service Communication

The Ordering Service uses **synchronous HTTP communication** to integrate with the Catalog Service.

### Flow during order creation:

1. User sends an order creation request with product IDs and quantities
2. Ordering Service calls **Catalog Service** to retrieve product details
3. Product name and price are copied into order items (snapshot)
4. Order is saved with calculated total amount

This approach ensures:

* Loose coupling between services
* Historical order accuracy even if product data changes later
* Simplicity suitable for MVP-scale microservices

---

## Endpoints

### Orders

| Method | Endpoint          | Role | Description                     |
| ------ | ----------------- | ---- | ------------------------------- |
| GET    | `/api/order`      | User | Get all orders for current user |
| GET    | `/api/order/{id}` | User | Get order by ID (user-scoped)   |
| POST   | `/api/order`      | User | Create a new order              |

> Authentication is handled via JWT issued by the Auth Service.\
> Orders are always scoped to the authenticated user.

---

## Validation

* All input DTOs are validated using **FluentValidation**
* Invalid requests return `400 Bad Request` with validation details
* Validation occurs at the controller boundary before business logic execution

---

## Technologies

* .NET 10 / ASP.NET Core Web API
* Entity Framework Core (Code-First)
* AutoMapper
* FluentValidation
* SQL Server
* Gridify (pagination & filtering)
* HTTPClientFactory (sync service-to-service communication)
* Docker

---

### ✅ MVP Status

This service is part of the **HealthStack MVP**, alongside:

* Auth Service
* Catalog Service

It demonstrates real-world backend patterns while remaining intentionally simple and extensible.