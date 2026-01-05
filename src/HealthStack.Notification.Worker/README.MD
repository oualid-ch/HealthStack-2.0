# HealthStack Notification Worker Service

The **Notification Worker Service** is a background worker responsible for consuming domain events and reacting to them asynchronously.

In the HealthStack MVP, this service listens for **OrderCreated** events and simulates sending order confirmation notifications.

This service demonstrates **event-driven architecture**, **message consumption**, and **eventual consistency** using RabbitMQ.

---

## Overview

* Background worker (no HTTP endpoints)
* Subscribes to RabbitMQ events
* Reacts independently to Order lifecycle events
* Decoupled from the Ordering Service

---

## Architecture


This service follows a **clean layered architecture**:

```
HealthStack.Notification.Worker
│
├─ Messaging # RabbitMQ consumer configuration
├─ Events # Event contracts (OrderCreated)
├─ Services # Background worker logic
└─ Program.cs # Worker startup
```

**Database:** SQL Server (local or Docker)
**Persistence:** EF Core (Code-First approach)

---

## Event Consumption

### Subscribed Event

**OrderCreatedEvent**

| Field        | Description                     |
|-------------|---------------------------------|
| OrderId     | Unique order identifier         |
| UserId      | User who placed the order       |
| TotalAmount | Order total amount              |
| CreatedAt   | Order creation timestamp        |

### Messaging Details

* **Exchange:** Topic exchange
* **Queue:** `notification.order.created.queue`
* **Routing Key:** `order.created`
* **Acknowledgement:** Automatic (MVP simplicity)

---

## Processing Flow

1. Ordering Service publishes `order.created` event
2. Notification Worker receives the message
3. Event payload is deserialized
4. A simulated notification is logged:

```text
[EMAIL] Order confirmation sent for Order {OrderId}
```

This approach demonstrates how new services can be added without modifying the Ordering Service.

---

## Design Notes

* The service intentionally avoids real email providers
* Focus is on infrastructure and communication patterns
* Retries, dead-letter queues, and advanced resiliency are omitted for MVP clarity

---

## Technologies

* .NET 10 Worker Service
* RabbitMQ
* Hosted BackgroundService
* Microsoft.Extensions.Hosting
* Docker

---

### MVP Purpose

This service exists to:

* Demonstrate asynchronous messaging
* Showcase event-driven design
* Validate RabbitMQ integration
* Provide a realistic but lightweight consumer example

It is intentionally simple and designed for learning and portfolio demonstration.