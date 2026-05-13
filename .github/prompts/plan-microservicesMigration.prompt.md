# Microservices Migration Plan
**Migration plan includes service decomposition, architecture, and step-by-step strategy**

---

## Overview

This plan outlines the migration of the monolithic ApiProject backend (Controllers, Services, Repositories) into a microservices architecture. The goal is to achieve independent scalability, deployment, and team ownership while maintaining system reliability.

**Current State**: Monolithic .NET 9.0 backend with shared database, single deployment unit.

**Target State**: Service-oriented architecture with independent services, API Gateway, service discovery, and polyglot persistence.

**Timeline**: Phased migration over 6-9 months (depending on team size and availability).

---

## Service Decomposition

Based on the current controller structure, decompose into these services:

### 1. **Authentication Service** (from `AuthController`)
- **Responsibility**: User authentication, token generation, identity validation
- **Operations**: Login, Register, Token Refresh, Validate Token
- **Database**: Dedicated auth database (PostgreSQL)
- **Exposed APIs**: REST endpoints + gRPC for internal token validation

### 2. **Category Service** (from `CategoryController`)
- **Responsibility**: Product/item categories, hierarchy management
- **Operations**: GetCategory, CreateCategory, UpdateCategory, DeleteCategory
- **Database**: PostgreSQL with dedicated schema
- **Dependencies**: None (independent)

### 3. **Gifts/Products Service** (from `GiftsController`)
- **Responsibility**: Gift/product management, inventory, pricing
- **Operations**: GetGift, CreateGift, UpdatePrice, ListGifts
- **Database**: PostgreSQL with dedicated schema
- **Dependencies**: Category Service, Inventory Service (for stock checks)

### 4. **Cart Service** (from `CartController`)
- **Responsibility**: Shopping cart operations
- **Operations**: AddToCart, RemoveFromCart, GetCart, ClearCart
- **Database**: Redis (cache-first approach) + PostgreSQL for persistence
- **Dependencies**: Gifts Service, Pricing Service

### 5. **Sales/Orders Service** (from `SalesController`)
- **Responsibility**: Order processing, sales records
- **Operations**: CreateOrder, GetOrder, CancelOrder, ListOrders
- **Database**: PostgreSQL with dedicated schema
- **Dependencies**: Cart Service, Gifts Service, Payment Service (external)
- **Transactions**: Saga-based transaction management

### 6. **Lottery Service** (from `LotteryController`)
- **Responsibility**: Lottery/raffle management, winner selection
- **Operations**: CreateLottery, DrawWinner, GetResults
- **Database**: PostgreSQL with dedicated schema
- **Dependencies**: Gifts Service, Sales Service

### 7. **Donors Service** (from `DonorsController`)
- **Responsibility**: Donor management, profiles, history
- **Operations**: GetDonor, CreateDonor, UpdateDonor, GetDonorHistory
- **Database**: PostgreSQL with dedicated schema
- **Dependencies**: Cart Service, Sales Service

---

## Architecture Design

### API Gateway Pattern
```
Client Layer
    ↓
[API Gateway] (Port 5000)
    ├─ /auth → Auth Service (5100)
    ├─ /categories → Category Service (5200)
    ├─ /gifts → Gifts Service (5300)
    ├─ /cart → Cart Service (5400)
    ├─ /sales → Sales Service (5500)
    ├─ /lottery → Lottery Service (5600)
    └─ /donors → Donors Service (5700)
```

**Implementation**: Use Kong, Ocelot, or AWS API Gateway
- Route requests to appropriate services
- Handle cross-cutting concerns: rate limiting, logging, authentication
- Validate JWT tokens (obtained from Auth Service)

### Communication Patterns

#### Synchronous (Request-Response)
- **REST over HTTP**: For simple queries (GetById, GetAll, Search)
- **gRPC**: For service-to-service calls needing low latency
  - Example: Cart Service → Gifts Service (get price)

#### Asynchronous (Event-Driven)
- **Message Broker**: RabbitMQ or Azure Service Bus
- **Events Published**:
  - `OrderCreated` → triggers Inventory update, Email notification
  - `PaymentProcessed` → triggers Order confirmation
  - `LotteryDrawn` → triggers Winner notification
- **Event Handlers**: Each service subscribes to relevant events

### Database per Service Pattern
```
Auth Service          → PostgreSQL (auth_db)
Category Service      → PostgreSQL (category_db)
Gifts Service         → PostgreSQL (gifts_db)
Cart Service          → Redis (carts) + PostgreSQL (persist)
Sales Service         → PostgreSQL (orders_db)
Lottery Service       → PostgreSQL (lottery_db)
Donors Service        → PostgreSQL (donors_db)
```

**Consistency Strategy**: Eventual consistency with event-driven updates. Use transaction IDs for correlation.

### Data Sharing & Joins
Since databases are decoupled, implement read-only replicas or cache popular data:
```csharp
// Example: Order including gift details
public class OrderDto
{
    public int OrderId { get; set; }
    public int GiftId { get; set; }
    public GiftCacheDto GiftDetails { get; set; }  // Cached data from Gifts Service
    public OrderStatus Status { get; set; }
}
```

---

## Service Discovery

- **Local Development**: Docker Compose with hardcoded service URLs
- **Production**: Kubernetes DNS or Consul
  - Service registration: Automatic on deployment
  - Health checks: Kubelet probes or Consul agent
  - Service lookup: Via DNS (e.g., `gifts-service:5300`)

**Implementation Example (Kubernetes)**:
```yaml
# Service definition
apiVersion: v1
kind: Service
metadata:
  name: gifts-service
spec:
  selector:
    app: gifts-service
  ports:
    - port: 5300
      targetPort: 5300
```

Clients call: `http://gifts-service:5300/api/gifts`

---

## Security & Authentication

### Token-Based Authentication
- Auth Service issues JWT tokens
- API Gateway validates token on each request
- Services trust tokens from API Gateway

### Implementation
```csharp
// Middleware in API Gateway and services
public class JwtMiddleware
{
    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        var token = context.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = 401;
            return;
        }

        var isValid = await authService.ValidateTokenAsync(token);
        if (!isValid)
        {
            context.Response.StatusCode = 403;
            return;
        }

        // Extract user claims and attach to context
        var claims = JwtSecurityTokenHandler.ReadJwtToken(token).Claims;
        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

        await _next(context);
    }
}
```

### Service-to-Service Communication
- Use mTLS (mutual TLS) for internal calls
- Or: Service-to-service tokens (short-lived, service-specific)
- Never expose sensitive data in inter-service calls

### API Rate Limiting
- Implement at API Gateway level
- Per-user limits (authenticated) and per-IP (public)
- Return 429 Too Many Requests when exceeded

---

## Migration Strategy

### Phase 1: Foundation (Weeks 1-2)
1. Set up DevOps infrastructure
   - Docker, Docker Compose, Kubernetes cluster (local + staging)
   - CI/CD pipeline (GitHub Actions or GitLab CI)
2. Create API Gateway service
   - Route all external traffic through gateway
   - Implement authentication middleware
3. Deploy existing monolith as first service (temporary)

### Phase 2: Extract Auth Service (Weeks 3-4)
1. Create new Auth Service project
2. Copy authentication logic from monolith
3. Deploy Auth Service independently
4. Update monolith to call Auth Service (internal)
5. Update API Gateway to route `/auth/*` to Auth Service

### Phase 3: Extract Stateless Services (Weeks 5-7)
Extract in order (dependencies first):
1. **Category Service** (no dependencies)
2. **Donors Service** (depends on Category indirectly)
3. **Gifts Service** (depends on Category)
4. **Lottery Service** (depends on Gifts)

For each service:
- Create new project, copy business logic
- Copy relevant DTOs, Models, Repositories
- Create dedicated database
- Implement data migration scripts
- Deploy alongside monolith
- Route requests via API Gateway
- Keep monolith as backup

### Phase 4: Extract Complex Services (Weeks 8-10)
1. **Cart Service**
   - Implement with Redis + PostgreSQL
   - Set up event publishing for cart events
2. **Sales/Orders Service**
   - Implement saga pattern for order transactions
   - Subscribe to Cart and Gifts events
   - Publish Order events

### Phase 5: Decompose Shared Concerns (Weeks 11-12)
1. Move logging to centralized ELK Stack (Elasticsearch, Logstash, Kibana)
2. Implement distributed tracing (Jaeger, Datadog)
3. Set up shared infrastructure:
   - Message broker (RabbitMQ)
   - Configuration server (Consul, App Configuration Service)
   - Service mesh (optional: Istio for traffic management)

### Phase 6: Cutover & Cleanup (Weeks 13+)
1. Traffic routing: 100% to microservices
2. Monitor for issues during peak hours
3. Decommission monolith
4. Archive monolith codebase

---

## Infrastructure & Deployment

### Containerization
Each service packaged as Docker image:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY bin/Release/net9.0/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "GiftsService.dll"]
```

Build and push to container registry (Docker Hub, ECR, ACR):
```bash
docker build -t myregistry/gifts-service:1.0.0 .
docker push myregistry/gifts-service:1.0.0
```

### Orchestration: Kubernetes
Deploy services as Kubernetes Deployments:
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: gifts-service
spec:
  replicas: 3
  selector:
    matchLabels:
      app: gifts-service
  template:
    metadata:
      labels:
        app: gifts-service
    spec:
      containers:
      - name: gifts-service
        image: myregistry/gifts-service:1.0.0
        ports:
        - containerPort: 5300
        env:
        - name: DATABASE_URL
          valueFrom:
            secretKeyRef:
              name: db-credentials
              key: gifts-db-url
        livenessProbe:
          httpGet:
            path: /health
            port: 5300
          initialDelaySeconds: 30
          periodSeconds: 10
```

### CI/CD Pipeline (GitHub Actions Example)
```yaml
name: Deploy Gifts Service

on:
  push:
    branches: [main]
    paths: ['GiftsService/**']

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      
      - name: Build Docker image
        run: |
          docker build -t myregistry/gifts-service:${{ github.sha }} GiftsService/
          docker push myregistry/gifts-service:${{ github.sha }}
      
      - name: Update Kubernetes deployment
        run: |
          kubectl set image deployment/gifts-service \
            gifts-service=myregistry/gifts-service:${{ github.sha }} \
            -n production
      
      - name: Wait for rollout
        run: kubectl rollout status deployment/gifts-service -n production
```

### Local Development: Docker Compose
```yaml
version: '3.8'
services:
  api-gateway:
    image: myregistry/api-gateway:latest
    ports:
      - "5000:5000"
    environment:
      - GIFTS_SERVICE_URL=http://gifts-service:5300
      - CART_SERVICE_URL=http://cart-service:5400

  gifts-service:
    image: myregistry/gifts-service:latest
    ports:
      - "5300:5300"
    environment:
      - DATABASE_URL=Server=postgres;Database=gifts_db;User=sa;Password=pass
    depends_on:
      - postgres

  postgres:
    image: postgres:16
    environment:
      - POSTGRES_PASSWORD=pass
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

---

## Monitoring & Logging

### Centralized Logging (ELK Stack)
All services log to Elasticsearch:
```csharp
builder.Services.AddSerilog(new LoggerConfiguration()
    .WriteTo.Elasticsearch("http://elasticsearch:9200")
    .MinimumLevel.Information()
    .CreateLogger());

// Usage in services
_logger.LogInformation("Processing order {OrderId}", orderId);
```

### Distributed Tracing (Jaeger)
Track requests across services:
```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(b => b
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddJaegerExporter(o =>
        {
            o.Endpoint = new Uri("http://jaeger:14250");
        }));
```

**Benefits**: See full request flow: API Gateway → Cart Service → Gifts Service → Database

### Health Checks
Each service exposes `/health` endpoint:
```csharp
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = WriteHealthCheckResponse
});

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database")
    .AddCheck<ExternalServiceHealthCheck>("external-api");
```

### Metrics & Alerts
Use Prometheus + Grafana:
```csharp
var counter = new Counter("orders_processed", "Total orders processed");
counter.Inc();

// Grafana dashboard displays:
// - Request latency per service
// - Error rates
// - Database connection pool usage
```

---

## Testing Strategy

### Unit Tests (Per Service)
```csharp
[TestClass]
public class GiftsServiceTests
{
    private Mock<IGiftsRepository> _mockRepo;
    private GiftsService _service;

    [TestInitialize]
    public void Setup()
    {
        _mockRepo = new Mock<IGiftsRepository>();
        _service = new GiftsService(_mockRepo.Object);
    }

    [TestMethod]
    public async Task CreateGift_WithValidDto_ReturnsCreatedGift()
    {
        var dto = new CreateGiftDto { Name = "Laptop", Price = 999 };
        var created = new Gift { Id = 1, Name = "Laptop", Price = 999 };

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Gift>()))
            .ReturnsAsync(created);

        var result = await _service.CreateAsync(dto);

        Assert.AreEqual(1, result.Id);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Gift>()), Times.Once);
    }
}
```

### Integration Tests (Service + Database)
```csharp
[TestClass]
public class GiftsServiceIntegrationTests
{
    private sealed class GiftsContextFactory : IDesignTimeDbContextFactory<ProjectContext>
    {
        public ProjectContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<ProjectContext>()
                .UseInMemoryDatabase("test_db")
                .Options;
            return new ProjectContext(options);
        }
    }

    [TestMethod]
    public async Task CreateAndRetrieveGift_ReturnsCorrectGift()
    {
        var context = new GiftsContextFactory().CreateDbContext(null);
        var repository = new GiftsRepository(context);
        
        var gift = new Gift { Name = "Book", Price = 25 };
        await repository.AddAsync(gift);

        var retrieved = await repository.GetByIdAsync(gift.Id);
        Assert.AreEqual("Book", retrieved.Name);
    }
}
```

### Contract/API Tests
Test service-to-service communication:
```csharp
[TestClass]
public class GiftsClientTests
{
    private HttpClient _httpClient;

    [TestInitialize]
    public void Setup()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("http://gifts-service:5300") };
    }

    [TestMethod]
    public async Task GetGift_ReturnsValidResponse()
    {
        var response = await _httpClient.GetAsync("/api/gifts/1");
        Assert.IsTrue(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsAsync<GiftDto>();
        Assert.IsNotNull(content);
    }
}
```

### End-to-End Tests (Full Flow)
Test via API Gateway:
```csharp
[TestMethod]
public async Task OrderFlow_CreateCartAddItemCheckout_Succeeds()
{
    // 1. Create cart
    // 2. Add gift to cart
    // 3. Create order from cart
    // 4. Verify order in Sales Service
    // 5. Verify cart is cleared
}
```

---

## Rollback & Disaster Recovery

### Blue-Green Deployment
```bash
# Deploy new version (green)
kubectl apply -f gifts-service-v2-deployment.yaml

# Run integration tests
./run-tests.sh

# If tests pass, switch traffic via load balancer
kubectl patch service gifts-service -p '{"spec":{"selector":{"version":"v2"}}}'

# If issues, switch back
kubectl patch service gifts-service -p '{"spec":{"selector":{"version":"v1"}}}'

# Clean up old version
kubectl delete deployment gifts-service-v1
```

### Database Rollback
```sql
-- Maintain backwards-compatible schema changes
-- Run migrations with rollback scripts

-- Example: nullable new columns before making them required
ALTER TABLE gifts ADD COLUMN new_field VARCHAR(100) NULL;
-- Backfill data
UPDATE gifts SET new_field = calculated_value;
-- Later: make NOT NULL
ALTER TABLE gifts ALTER COLUMN new_field VARCHAR(100) NOT NULL;
```

### Communication During Incident
- Use feature flags to disable problematic service calls
- Route traffic to v1 if v2 has critical bugs
- Maintain fallback mechanisms for external service failures

---

## Example: Order Flow in Microservices Architecture

```
Client Request: POST /api/sales/orders
        ↓
[API Gateway] (Validates JWT)
        ↓
[Sales Service]
        ├─ Call Cart Service (REST): GET /api/cart/{userId}
        │   └─ Returns: [{ giftId: 1, quantity: 2 }]
        │
        ├─ Call Gifts Service (gRPC): GetGiftPrice(giftId: 1)
        │   └─ Returns: { giftId: 1, price: 99.99 }
        │
        ├─ Calculate total, create order in DB
        │   └─ Order saved: { id: 101, total: 199.98, status: PENDING }
        │
        ├─ Publish Event: OrderCreated (to RabbitMQ)
        │   └─ Event consumed by:
        │       ├─ Email Service → sends confirmation
        │       ├─ Inventory Service → updates stock
        │       └─ Analytics Service → logs order
        │
        └─ Return: { orderId: 101, status: 201 Created }
```

---

## Key Best Practices

✅ **Loose Coupling**: Services communicate via APIs/events, not shared databases
✅ **Single Responsibility**: Each service owns one domain (Gifts, Cart, Orders, etc.)
✅ **Independent Deployability**: Deploy services without coordinating with others
✅ **Fault Isolation**: Service failure doesn't crash entire system (circuit breakers)
✅ **API Versioning**: Support v1 and v2 simultaneously during migration
✅ **Backward Compatibility**: Don't break existing API contracts
✅ **Async Operations**: Use message brokers for non-critical flows
✅ **Database per Service**: Avoid shared databases to prevent tight coupling
✅ **Observability**: Centralized logging, tracing, and metrics

---

## Estimated Effort & Timeline

| Phase | Duration | Effort | Key Deliverables |
|-------|----------|--------|------------------|
| Foundation | 2 weeks | 40 hours | API Gateway, CI/CD, Kubernetes cluster |
| Auth Service | 2 weeks | 30 hours | Auth Service deployed, token validation working |
| Stateless Services | 3 weeks | 90 hours | Category, Donors, Gifts, Lottery services live |
| Complex Services | 3 weeks | 80 hours | Cart, Sales services with saga pattern |
| Infrastructure | 2 weeks | 50 hours | Logging, tracing, monitoring, alerting |
| Testing & Cutover | 2 weeks | 60 hours | Full integration tests, performance tuning, go-live |
| **Total** | **14 weeks** | **~350 hours** | Full microservices architecture |

**Team size assumption**: 5 engineers working part-time on migration (alongside maintenance).

---

## Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|-----------|
| Network latency between services | Performance degradation | Use gRPC for internal calls, implement caching |
| Data consistency issues | Audit/sync problems | Use event sourcing, implement saga orchestration |
| Operational complexity | DevOps overhead | Invest in Kubernetes, observability, automation |
| Team coordination | Deployment conflicts | Clear service ownership, feature flags, versioning |
| Legacy code dependencies | Difficult to extract | Create adapters, gradually refactor, maintain monolith temporarily |

---

## Next Steps

1. **Review & Approve**: Get stakeholder sign-off on this plan
2. **Set Up Foundation**: Create DevOps infrastructure (Docker, Kubernetes, CI/CD)
3. **Extract Auth Service**: First concrete service, test automation/deployment pipeline
4. **Iterate**: Extract remaining services in dependency order
5. **Monitor & Optimize**: Continuously tune performance and reliability

---

**Document Version**: 1.0  
**Last Updated**: February 26, 2026  
**Status**: Ready for implementation
