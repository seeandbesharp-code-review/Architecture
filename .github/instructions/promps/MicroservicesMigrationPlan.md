# Microservices Migration Plan — Summary

> Full detailed plan: `.github/prompts/plan-microservicesMigration.prompt.md`

---

## Status: Planning Only — Not Implemented

The current backend is a monolithic ASP.NET Core 8.0 application.  
This document is a planning artifact only. No migration has begun.

---

## Target: 7 Independent Services

| Service | Source Controller | Port | Database |
|---------|------------------|------|----------|
| Auth Service | `AuthController` | 5100 | PostgreSQL (auth_db) |
| Category Service | `CategoryController` | 5200 | PostgreSQL (category_db) |
| Gifts Service | `GiftsController` | 5300 | PostgreSQL (gifts_db) |
| Cart Service | `CartController` | 5400 | Redis + PostgreSQL |
| Sales Service | `SalesController` | 5500 | PostgreSQL (orders_db) |
| Lottery Service | `LotteryController` | 5600 | PostgreSQL (lottery_db) |
| Donors Service | `DonorsController` | 5700 | PostgreSQL (donors_db) |

All services sit behind a single **API Gateway** on port 5000.

---

## Migration Order (by dependency)

1. Auth Service (no deps)
2. Category Service (no deps)
3. Donors Service
4. Gifts Service (depends on Category)
5. Lottery Service (depends on Gifts)
6. Cart Service (depends on Gifts)
7. Sales Service (depends on Cart + Gifts)

---

## Key Architecture Decisions

- **Database per service** — no shared DB, eventual consistency via events
- **Sync communication**: REST (simple queries), gRPC (low-latency service-to-service)
- **Async communication**: RabbitMQ or Azure Service Bus for events (`OrderCreated`, `LotteryDrawn`, etc.)
- **Auth**: JWT issued by Auth Service, validated by API Gateway — downstream services trust the gateway
- **Deployment**: Docker + Kubernetes; CI/CD via GitHub Actions
- **Observability**: ELK Stack (logging), Jaeger (tracing), Prometheus + Grafana (metrics)

---

## Estimated Effort

~14 weeks, ~350 hours for a team of 5 engineers part-time.

See the [full plan](.././prompts/plan-microservicesMigration.prompt.md) for phase-by-phase breakdown, code examples, Kubernetes manifests, CI/CD pipelines, rollback strategy, and risk analysis.
