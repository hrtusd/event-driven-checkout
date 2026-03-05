# EventDrivenCheckout — Running & Testing

## Prerequisites

- Visual Studio 2026 (latest)
- .NET Aspire workload installed
- Docker Desktop running

## Running the Project

1. Set **EventDrivenCheckout.AppHost** as the startup project
2. Press **F5** (or Run)

Aspire will start all services and open the Aspire dashboard automatically. Docker Desktop handles RabbitMQ and SQL Server containers — these are spun up automatically on first run.

## Testing

Open `EventDrivenCheckout.Basket.http` in Visual Studio. Run the requests in order:

**1. Add items to the basket**

Run requests 1 and 2 to populate basket with products.

**2a. Normal checkout**

Run `3a. Checkout` — this triggers the full happy path:

```
BasketCheckedOut → OrderAcceptedV2 → ShipmentRepriced → OrderConfirmed
```

**2b. Checkout with simulated failure**

Run `3b. Checkout with simulated failure` instead — `triggerFailure: true` is passed through the saga and causes the logistics service to publish `ShipmentFailed`, exercising the compensation path:

```
BasketCheckedOut → OrderAcceptedV2 → ShipmentFailed → OrderCancelled
```

> Run steps 1 and 2 again between each test — the basket is cleared on checkout, and each checkout creates a new order with a fresh `OrderId`.

## Observing the Flow

- **Aspire Dashboard** — structured logs from all services with state transitions visible in real time, request tracing across services, and the ability to inspect messages and state changes in depth
- **RabbitMQ Management UI** — default username is guest and password as well as the URL is located in the resource properties in Aspire dashboard.