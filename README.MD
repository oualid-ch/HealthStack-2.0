# HealthStack

**HealthStack** is a microservices-based eCommerce backend platform focused on health products.

This repository showcases real-world backend engineering practices using **.NET 10**, **ASP.NET Core**, **Entity Framework Core**, **JWT authentication**, and both **synchronous and asynchronous service communication** via an **API Gateway (YARP)**.

The goal of this project is to demonstrate **clean architecture**, **microservices fundamentals**, and **production-ready patterns** while keeping the scope intentionally small and MVP-focused.

---

## Architecture Overview

HealthStack follows a **microservices architecture** with clear service boundaries, independent data stores, and a **reverse proxy gateway** as a single entry point.

### Services

| Service | Description |
|-------|-------------|
| [Auth Service](./src/HealthStack.Auth.Api/README.md) | User registration, login, JWT authentication, and role management |
| [Catalog Service](./src/HealthStack.Catalog.Api/README.md) | Product and category management |
| [Order Service](./src/HealthStack.Order.Api/README.md) | Order creation, retrieval, and event publishing |
| [Notification Worker](./src/HealthStack.Notification.Worker/README.md) | Background worker consuming order events |
| [API Gateway (YARP)](./src/HealthStack.Gateway) | Single entry point routing requests to microservices |

---

## Communication Patterns

### Authentication (JWT)

1. User registers or logs in via **Auth Service**  
2. Auth Service issues a **JWT**  
3. Other services validate JWT via a shared secret and extract user identity & role  

All service endpoints behind the **API Gateway** require JWT for authenticated operations.

---

### Synchronous Communication (HTTP)

* **Order Service → Catalog Service**  
* Used during order creation to fetch product name and price  
* Product data is **snapshotted** into the order for historical accuracy  

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

## API Gateway Routing (YARP)

The **API Gateway** exposes routes for all services:

| Path | Forwarded To |
|------|--------------|
| `/auth/...` | Auth Service |
| `/catalog/...` | Catalog Service |
| `/order/...` | Order Service |

Transforms strip the service prefix so that microservices receive clean paths.

---

## Key Design Principles

* Clear **service boundaries**  
* Independent database per service  
* DTO-based communication between services  
* Snapshotting external data for historical accuracy  
* Clean layered architecture (Controllers → Services → Data/Clients)  
* Minimal but realistic infrastructure (Docker + RabbitMQ + SQL Server)  
* Learning-focused MVP design  

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