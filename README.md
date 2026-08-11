# RushTodo

Small, polished technical-interview vertical slice for managing gardeners' work items.

The intended solution shape is:

```text
RushTodo.Api
RushTodo.Database
rush-todo-web
RushTodo.UnitTests
RushTodo.IntegrationTests
```

The solution currently contains the Web API shell with EF Core persistence mapping, persisted entities, SQL Server database project, and documentation. Remaining projects will be added one focused increment at a time.

## Start Here

- `CODEX-BRIEF.md` is the product and implementation brief, including the backlog and domain/API contract.
- `README.md` records the project conventions and development setup.
- `AGENTS.md` records Codex/session behaviour and guardrails.

## Development Setup

Use Visual Studio with:

- the **ASP.NET and web development** workload for the API and React tooling;
- the **Data storage and processing** workload, or SQL Server Data Tools, for `RushTodo.Database.sqlproj` and DACPAC deployment;
- the .NET SDK selected when the API projects are created;
- SQL Server/LocalDB available for local development and integration tests;
- Node.js LTS for the Vite React application.

ReSharper is required to maintain the ReSharper-Green coding standard. The solution-level `RushTodo.sln.DotSettings` and repository `.editorconfig` carry the shared standards. Personal `.DotSettings.user` files are ignored by Git.

The future SQL Project may not build with the plain .NET CLI because it uses SSDT targets. Use Visual Studio for DACPAC builds/deployment; use focused project builds for API/client verification when appropriate.

## Architecture Principles

- SQL Server schema lives in the SQL Project and deploys through a DACPAC; EF migrations are not the authority.
- Post-deploy seed scripts contain required reference data only. Optional/bootstrap business data belongs in explicitly run scripts under `RushTodo.Database/Scripts/Upgrade`.
- The API is a conventional C#/.NET HTTP boundary. Controllers are thin; services own business operations; repositories own EF persistence and caching.
- Reuse the proven VeggieCoOp foundations: entity/model mapping, `ServiceBase`, `BaseRepository`/`StaticRepository`, transaction orchestration, auditing, validation, and optimistic concurrency.
- Concrete keys are `WorkItemId` and `GardenerId`; shared entity/model abstractions expose `Id` and `UpdateDateTime` for generic infrastructure.
- Small reference entities such as `EntityType` and `WorkItemStatus` inherit from `Enummy`. Like `Entity.Id`, their `int Id` is an unmapped alias over an explicit enum-typed mapped key such as `WorkItemStatusId`; this keeps generic lookup dictionaries simple without discarding domain typing. Enummy properties are read-only to application code, expose an explicitly named display property for queries/selectors, and do not carry irrelevant entity timestamps.
- `UpdateDateTime` is a UTC `DATETIME2` optimistic-concurrency token. Reject stale writes centrally and translate them in API exception middleware to `409 Conflict`.
- `DateOnly` is for business dates and maps to SQL `DATE`; genuine instants are UTC `DateTime` values from `IDateTimeService.UtcNow`.
- A work item may be unassigned, so `GardenerId` is nullable. Cancellation is a WorkItem business operation and remains distinct from soft deletion.
- Until authentication and authorization are implemented, auditing uses one seeded development `AppUser` with the stable ID `1`. Replace this with authenticated user provisioning when that work begins.
- React is TypeScript/Vite with app-owned CSS and shared controls—not Bootstrap. UI code calls one API facade rather than scattering `fetch`.

## Coding Standards

- The basic coding standard is ReSharper-Green: the ReSharper settings have been configured to ensure consistency and desired standards.
- Keep code simple, explicit, and readable; introduce abstractions when they remove real duplication or clarify a workflow.
- Interfaces normally sit above their implementation in the same file.
- Prefer arrays over lists in public APIs unless callers need mutation.
- Use `""` rather than `string.Empty`; private fields are `_camelCase`; interfaces use the `IName` convention.
- `Task`-returning methods do not need an `Async` suffix.
- Keep controllers free of business logic and keep exception handling centralised in middleware.
- Do not silently overwrite newer data: all update/state-change commands use the supplied concurrency version.
- Use CSS variables/shared components for reusable visual behaviour; keep React components small with ordinary explicit state and data flow.
- Keep changes in small, reviewable increments. Do not progress to another backlog item without explicit direction.
