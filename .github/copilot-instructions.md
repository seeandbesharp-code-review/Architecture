# AI Assistant Instructions — Gift Lottery Platform

## Project Overview

A **Lottery-based Gift Sales Platform** with a full-stack architecture:
- **Frontend**: Angular 21 (TypeScript), located in `AngularProject2/AngularProject/`
- **Backend**: ASP.NET Core 8.0 (.NET 9.0), C# — located in `ApiProject2/ApiProject/`
- **Database**: SQL Server via Entity Framework Core 9.0

## Domain Logic

1. **Donors** contribute gifts to the system
2. **Gifts** are organized by **Categories** and carry a ticket price
3. **Users** add gifts to a **Cart** and purchase them (each purchase = lottery tickets)
4. **Cart** tracks items and quantities; purchasing marks the cart as paid
5. **Sales** reports aggregate purchase summaries across gifts
6. **Lottery** draws a winner from all purchasers, weighted by ticket count

## Backend Architecture

Strict layered architecture — never skip a layer:

```
HTTP Request
    ↓
Controller        (ApiProject/Controllers/)
    ↓
Service           (ApiProject/Services/Interface/ + Implement/)
    ↓
Repository        (ApiProject/Repositories/Interface/ + Implement/)
    ↓
EF Core → SQL Server
```

### Key Patterns
- **Repository Pattern** — all DB access behind `IXxxRepository` interfaces
- **Service Layer** — all business logic in `IXxxService` implementations; controllers only call services
- **DTO Pattern** — controllers receive and return DTOs, never raw Models
- **Dependency Injection** — constructor injection throughout; register in `Program.cs`

### Folder Structure
```
ApiProject2/ApiProject/
├── Controllers/       → HTTP endpoints (see .github/instructions/controllers.instructions.md)
├── Services/
│   ├── Interface/     → IXxxService.cs
│   └── Implement/     → XxxService.cs
├── Repositories/
│   ├── Interface/     → IXxxRepository.cs
│   └── Implement/     → XxxRepository.cs  (see .github/instructions/repositories.instructions.md)
├── Models/            → EF Core entity classes
├── DTO/               → Data transfer objects (input/output shapes)
├── Data/              → ProjectContext.cs (DbContext), ProjectContextFactory.cs
├── MiddleWare/        → JWT validation, global error handling
├── Migrations/        → EF Core migration files
├── Reports/           → Report generation
└── Program.cs         → DI registration, middleware pipeline
```

## Tech Stack

| Concern | Technology |
|---------|-----------|
| Backend | ASP.NET Core 8.0 |
| ORM | Entity Framework Core 9.0 |
| Database | SQL Server |
| Auth | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`) |
| Passwords | BCrypt.Net-Next |
| Caching | Redis (`StackExchange.Redis`) |
| Logging | Serilog + Serilog.AspNetCore |
| Email | MailKit / MimeKit |
| API Docs | Swagger (Swashbuckle.AspNetCore) |
| Frontend | Angular 21 |
| Containerization | Docker (Linux containers) |

## Coding Conventions

- **Language for error messages**: Hebrew (e.g., `"המשתמש לא נמצא"`, `"המוצר כבר קיים"`)
- All repository and service methods are `async`/`await` — return `Task<T>`
- Use `_logger.LogInformation()` / `_logger.LogError()` in every method (Serilog)
- Validate inputs at the **service layer**, not in controllers
- Use `.Include()` / `.ThenInclude()` for eager loading in repositories — avoid lazy loading
- Repository methods return domain **Models**; services map to **DTOs** before returning to controllers
- Keep controllers thin: validate input → call service → return HTTP result

## Reference Files

> Load these files only when working on the relevant area to avoid wasting tokens:

- **Controllers** (routes, methods, DTOs): `.github/instructions/controllers.instructions.md`
- **Repositories** (interfaces, method signatures): `.github/instructions/repositories.instructions.md`
- **Microservices migration plan**: `.github/prompts/plan-microservicesMigration.prompt.md`