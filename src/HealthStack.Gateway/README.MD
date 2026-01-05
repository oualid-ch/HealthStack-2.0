# HealthStack Gateway (YARP)

The **HealthStack Gateway** is the single entry point for the HealthStack microservices system. It is implemented using **YARP (Yet Another Reverse Proxy)** and is responsible for routing incoming HTTP requests to the appropriate backend microservice.

This gateway enables:

* Centralized routing
* A single public-facing endpoint
* Cleaner internal networking between services
* Easier security, logging, and rate limiting in the future

---

## Architecture Overview

```
Client
  |
  v
Gateway (YARP)  ← only exposed service
  |
  +--> Auth Service
  +--> Catalog Service
  +--> Order Service
```

All services communicate internally over Docker networking using service names.

---

## Routing Rules

The Gateway uses **path-based routing**.

| Public Path  | Target Service | Internal Address      |
| ------------ | -------------- | --------------------- |
| `/auth/*`    | Auth API       | `http://auth:8080`    |
| `/catalog/*` | Catalog API    | `http://catalog:8080` |
| `/order/*`   | Order API      | `http://order:8080`   |

### Example

```http
GET http://localhost:5000/order/api/Order
```

This request is proxied internally to:

```http
http://order:8080/api/Order
```

---

## Configuration

Gateway routing is configured via **appsettings.json** using YARP’s configuration-based model.

### appsettings.json

```json
{
  "ReverseProxy": {
    "Routes": {
      "auth": {
        "ClusterId": "auth-cluster",
        "Match": { "Path": "/auth/{**catch-all}" }
      },
      "catalog": {
        "ClusterId": "catalog-cluster",
        "Match": { "Path": "/catalog/{**catch-all}" }
      },
      "order": {
        "ClusterId": "order-cluster",
        "Match": { "Path": "/order/{**catch-all}" }
      }
    },
    "Clusters": {
      "auth-cluster": {
        "Destinations": {
          "auth": { "Address": "http://auth:8080/" }
        }
      },
      "catalog-cluster": {
        "Destinations": {
          "catalog": { "Address": "http://catalog:8080/" }
        }
      },
      "order-cluster": {
        "Destinations": {
          "order": { "Address": "http://order:8080/" }
        }
      }
    }
  }
}
```

---

## Running the Gateway

The Gateway is run as part of the Docker Compose stack.

### Docker Compose excerpt

```yaml
gateway:
  build:
    context: ./src/HealthStack.Gateway
    dockerfile: Dockerfile
  ports:
    - "5000:8080"
  depends_on:
    - auth
    - catalog
    - order
  networks:
    - appnet
```

Once running, the Gateway is available at:

```
http://localhost:5000
```

---

## Security Notes

* The Gateway currently **forwards requests transparently**.
* JWT validation is handled inside individual microservices.
* The Gateway can later be extended to:

  * Validate JWT tokens centrally
  * Enforce rate limits
  * Apply CORS policies
  * Add retries and circuit breakers

---

## Best Practices Enforced

* Only the Gateway exposes a public port
* Microservices are accessible **only via Docker networking**
* No hardcoded `localhost` references between services

---

## Future Enhancements

Planned improvements:

* JWT validation at the Gateway level
* Centralized logging and request tracing
* Rate limiting and throttling
* API versioning
* Health check aggregation
