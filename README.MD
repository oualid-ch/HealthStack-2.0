# HealthStack

**HealthStack** is a microservices-based eCommerce backend platform focused on health products.

This repository showcases real-world backend engineering practices using **.NET 10**, **ASP.NET Core**, **Entity Framework Core**, **JWT authentication**, and both **synchronous and asynchronous service communication**.

The goal of this project is to demonstrate **clean architecture**, **microservices fundamentals**, and **production-ready patterns** while keeping the scope intentionally small.

---

## Architecture Overview

HealthStack follows a **microservices architecture** with clear service boundaries and independent data stores.

### Services

| Service | Description |
|-------|-------------|
| [Auth Service](./src/HealthStack.Auth.Api/README.md) | User registration, login, JWT authentication, and role management |
| [Catalog Service](./src/HealthStack.Catalog.Api/README.md) | Product and category management |
| [Order Service](./src/HealthStack.Order.Api/README.md) | Order creation, retrieval, and event publishing |
| [Notification Worker](./src/HealthStack.Notification.Worker/README.md) | Background worker consuming order events |

---

## Communication Patterns

### Authentication (JWT)

1. User registers or logs in via **Auth Service**
2. Auth Service issues a **JWT**
3. Other services validate JWT and extract user identity & role

---

### Synchronous Communication (HTTP)

* **Order Service → Catalog Service**
* Used during order creation to fetch product name and price
* Product data is snapshotted into the order for historical accuracy

---

### Asynchronous Communication (Events)

* **Order Service → RabbitMQ → Notification Worker**
* Order Service publishes an `OrderCreated` event after persistence
* Notification Worker consumes the event and simulates sending a notification

This demonstrates:
- Event-driven architecture
- Loose coupling
- Eventual consistency

---

## Key Design Principles

* Clear service boundaries
* Database per service
* DTO-based communication
* Snapshotting external data
* Clean layered architecture
* Minimal but realistic infrastructure
* Learning-focused implementation

---

## Getting Started

### Requirements
* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* SQL Server (local or Docker)
* Docker & Docker Compose (optional, recommended)
* RabbitMQ (via Docker)

---

## Author

Built by [**Oualid Chelghoum**](https://github.com/oualid-ch)
Fullstack software engineer