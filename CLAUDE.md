# ECommerce API

ASP.NET Core Web API (.NET 10), Clean Architecture. Solution: ECommerce.slnx.

## Projects
- ECommerce.Domain: entities, Result/Error types, static error catalogs (Domain/Errors), repository + ISpecification interfaces. No project dependencies.
- ECommerce.UseCases: CQRS requests and handlers on a manual mediator (ISender, IRequestHandle, IPipelineBehavior, ValidationBehavior), FluentValidation validators, DTOs, Specification<T> and builders.
- ECommerce.Infrastructure: StoreDbContext + AppIdentityDbContext (same database, separate migration history tables), repositories, UnitOfWork, SpecificationEvaluator, Identity, JWT, refresh tokens, HybridCache/Redis (basket), Stripe, Cloudinary, seeding.
- ECommerce.API: minimal API endpoint groups (Endpoints/), API versioning, Swagger, middleware, filters.
- ECommerce.ArchitectureTests: architecture tests (currently no tests; verify).

## Conventions
- Expected failures return Result/Result<T> with an Error from the catalogs. Exceptions only for programmer errors and at infrastructure boundaries (Redis, Stripe).
- One folder per request (vertical slice): Command/Query + Handler. Add a validator only when the request needs one.
- Queries inject IReadRepository<T>; commands inject IRepository<T> + IUnitOfWork.
- Specification for non-trivial queries and projections; plain GetByIdAsync for trivial lookups (intentional).
- Endpoints are static Map*Endpoints(ApiVersionSet) classes, routes /api/v{version}/..., responses go through FromResult / FromPagedResult / Problem extensions (ApiResponse<T>).
- Dependencies point inward: Domain <- UseCases <- Infrastructure <- API (API references Infrastructure only for DI registration).
- The basket lives in HybridCache/Redis (IBasketStore), not SQL. Order.UserId is a logical FK (identity is a separate DbContext).

## Intentional decisions (do not "fix" without asking)
- Manual mediator instead of MediatR.
- The Stripe webhook reads the raw request body; never switch it to [FromBody] (breaks signature verification).
- Refresh tokens are stored hashed only; rotation + reuse detection live in RefreshTokenService.
- Payment amount is computed server-side from order.Total, never from client input.
- JWT ClockSkew = 0.

## Known gaps (the audit list)
- Verify Identity is registered once (AddIdentityCore in AddInfrastructure), not also in AddPresentation.
- Webhook idempotency, rate limiting on auth endpoints, real email sending, tests, Serilog, health checks, CI.

## Commands (confirm paths match the Src folder)
- Build: dotnet build ECommerce.slnx
- Run: dotnet run --project Src/ECommerce.API
- Test: dotnet test ECommerce.slnx

## Secrets
- Secrets live in dotnet user-secrets (Jwt:Secret, Stripe:*, CloudinarySettings:*, Seed:SuperAdmin:Password, connection strings with passwords).
- Never put real secrets in appsettings*.json or any committed file.
- Never print or log secret values.

## Rules for the agent
- Do not change public API contracts or database migrations without asking.
- Make one focused change at a time, then run build and tests.
- Explain each change briefly so I can understand and defend it in an interview.
- Reply in Egyptian Arabic mixed with English technical terms.
- Do not commit; I review the diff and commit myself.