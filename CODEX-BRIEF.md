# Rush Technical Interview Preparation

## Context and Objective

I need to prepare a small working application for a technical interview. The supplied brief deliberately describes substantially more work than can reasonably be completed in the available preparation time. I have approximately two hours available for the initial implementation.

The objective is **not to attempt to complete the entire brief**. Instead, build a small, polished, working vertical slice that demonstrates the engineering approaches, patterns and experience I already have.

Prioritise a working end-to-end application; clean code I can confidently explain and modify live; established architectural, database and testing patterns; and a simple React implementation demonstrating how I can use AI effectively to become productive quickly in a less-familiar technology. Keep the implementation deliberately small. Do not over-engineer it.

## Use VegiCoop as the Reference Implementation

Before creating or modifying code, inspect my existing VegiCoop solution carefully. Use it as the primary reference for my established coding style, architectural preferences and implementation patterns. Prefer adapting existing patterns rather than introducing new libraries, abstractions or architectural approaches.

In particular, identify and reuse where appropriate: Entity Framework/DbContext patterns; repositories including transactional vs cached reference data; services and DI; entity/model separation and mapping; validation; optimistic concurrency; soft delete; auditing; exception handling; helpers/extensions; SQL Project/DACPAC deployment; `.editorconfig`, ReSharper and other coding standards; and user-context/date-time-service patterns.

Do not blindly copy irrelevant functionality. Infer my broader preferences: simple, explicit, readable code; avoid unnecessary abstractions; centralise infrastructure rather than repeating it in business classes; keep business logic out of controllers.

## Solution Structure

```text
RushTodo.Api
RushTodo.Database
rush-todo-web
RushTodo.UnitTests
RushTodo.IntegrationTests
```

`RushTodo.Api`:

```text
Controllers/
Services/
Repositories/
Entities/
Models/
Mappers/
Validators/
Helpers/
Program.cs
```

Controllers are thin HTTP adapters. Real work belongs in services. Persistence/caching belongs in repositories. Keep service/repository interfaces in the same file as their implementation unless there is a strong reason not to. Do not create `Interfaces`, `Infrastructure`, `Data`, etc. merely for symmetry. `Helpers` may contain extensions, utility classes and small shared exception types such as concurrency exceptions. Enable Swagger/OpenAPI.

## Domain Model

### WorkItem

```csharp
WorkItem
{
    int Id
    string Title
    string Description
    WorkItemStatusId StatusId
    string Address
    int? GardenerId
    DateOnly? ScheduledDate
    DateOnly? CompletionDate
    DateOnly? CancellationDate
    DateTime UpdateDateTime
    bool IsDeleted
}
```

`Id` is an integer identity. `Description` maps to `varchar(max)`. Scheduled/completion/cancellation values are nullable business dates. `UpdateDateTime` is optimistic concurrency. `DateTime` represents an actual instant and is UTC by convention—do not suffix `Utc`. `DateOnly` represents a calendar/business date and must not be converted to UTC or arbitrary midnight DateTimes. Creation/history auditing comes from the VegiCoop auditing pattern; do not add `CreateDateTime`. `IsDeleted` is for administrative/data-retention concerns; cancellation is a business operation.

```csharp
enum WorkItemStatusId
{
    New = 1,
    Scheduled = 2,
    Done = 3,
    Cancelled = 4
}
```

### Gardener

```csharp
Gardener
{
    int Id
    string Name
    string PhoneNumber
    string? Email
    DateTime UpdateDateTime
    bool IsDeleted
}
```

Phone is required; email optional. A WorkItem may be unassigned or have one Gardener. Gardener is a domain entity, not an authentication identity. No gardener-management UI is required.

## Database

Use SQL Server. `RushTodo.Database` is a SQL Project and the authoritative schema definition. Follow VegiCoop's SQL Project/DACPAC approach. Create singular `Gardener` and `WorkItem` tables, enummy `EntityType` and `WorkItemStatus` reference tables, relevant auditing support, and appropriate FKs/constraints/lengths. Do not use EF migrations as the authoritative deployment mechanism.

Provide a local development deployment profile following VegiCoop and a separate integration-test profile that recreates the integration-test database. Post-deploy scripts seed only required reference data; manually run upgrade scripts may initialise local business data. Integration-test infrastructure owns all deterministic test business data.

## Repositories and Services

Follow VegiCoop patterns. Create `WorkItemRepository` and `GardenerRepository`; Gardener data may be cached as relatively static reference data, while WorkItems are transactional and query SQL Server directly. Create `IWorkItemService`/`WorkItemService` and `IGardenerService`/`GardenerService`, with each interface in the same file as its implementation. Controllers call services only.

## WorkItem Search

```csharp
public class WorkItemSearch
{
    public int[]? GardenerIds { get; set; }
    public DateOnly? ScheduledFrom { get; set; }
    public DateOnly? ScheduledTo { get; set; }
    public WorkItemStatusId[]? StatusIds { get; set; }
    public bool IncludeDeleted { get; set; }
}
```

```csharp
Task<IReadOnlyList<WorkItemModel>> SearchAsync(
    WorkItemSearch search,
    CancellationToken cancellationToken = default);
```

The date range deliberately uses `DateOnly`. The initial React UI need not expose all search options; this is a useful API and integration-testing surface.

## User Context and Date/Time Handling

Reuse VegiCoop's user-context and date/time-service patterns. User context includes time zone; default to New Zealand time. Reuse the date/time service instead of scattering `DateTime.UtcNow`. Persist genuine DateTimes as UTC without `Utc` suffixes. Use `DateOnly` for business dates and never timezone-convert them. Centralise genuine timezone conversion.

## API Contract

Use singular controller/resource names:

```text
GET    /work-item/{id}
POST   /work-item
PUT    /work-item/{id}
POST   /work-item/{id}/cancel
POST   /work-item/search
GET    /gardener
GET    /health
```

Use `WorkItemController` and `GardenerController`. Search is POST deliberately because the contract contains arrays and may evolve. Controllers remain thin. Enable Swagger/OpenAPI.

### Optimistic Concurrency

Use `UpdateDateTime` as the concurrency token. The client returns the version it received. On stale update/state change, reject it using the VegiCoop concurrency-exception pattern and return an appropriate HTTP conflict. Never silently overwrite newer data. Keep concurrency centralised below controllers.

### Cancellation

Cancellation is a business operation, not deletion. Set `StatusId = WorkItemStatusId.Cancelled` and `CancellationDate` through the service layer. Gardeners retain `IsDeleted`, although gardener CRUD/delete endpoints are not required.

## Validation

Create WorkItem write validation only; no Gardener validator. Title and Address are required with sensible maximum lengths; GardenerId is optional; Description optional; Scheduled requires `ScheduledDate`; Done requires `CompletionDate`; Cancelled requires `CancellationDate`. Keep validators simple and dependency-free where practical. Backend validation is authoritative. React may duplicate simple UX validation, but do not build validation code generation now.

## React UI

Use React + TypeScript + Vite + Bootstrap. Inspect VegiCoop's UI and reproduce its general feel/interaction patterns where natural. Infer my style and apply idiomatic React equivalents. Keep it straightforward and easy to modify live. Do not introduce Redux/global state management; use ordinary React state and explicit data flow.

Home page: table of incomplete WorkItems, suggested columns Title, Gardener, Address, Scheduled Date, Status, Actions. Sort sensibly by scheduling. Filtering by Gardener/Status and simple sorting are optional later additions, not blockers.

Provide Add and row Edit using the same dialog/modal. Fields: Title, Description, Gardener, Address, Status, Scheduled Date, Completion Date, Cancellation Date where appropriate. Populate Gardeners from the API. Provide row Cancel with confirmation where appropriate.

## React API Facade

Create one simple TypeScript API facade so UI code uses operations conceptually like:

```typescript
Api.getGardeners()
Api.searchWorkItems(search)
Api.getWorkItem(id)
Api.saveWorkItem(workItem)
Api.cancelWorkItem(id, ...)
```

Keep verbs, URLs, JSON serialization/deserialization and common errors inside this layer rather than scattering `fetch`. Prefer generated TypeScript API models/client from Swagger if simple/reliable, but hide generated details behind the facade. If generation becomes a time sink, use a small explicit API layer instead.

Use strict TypeScript, ESLint/consistent formatting, small readable components, Bootstrap, minimal dependencies, no `any` unless unavoidable, and no unnecessary advanced React abstractions. Add brief comments for unfamiliar React-specific constructs where useful.

## Integration Tests

Use NUnit against a real SQL Server database created from the DACPAC. The integration-test deployment profile recreates schema and leaves it empty. Use NUnit project/namespace-level `[SetUpFixture]` with `[OneTimeSetUp]` for global setup.

```text
RushTodo.IntegrationTests/
    TestData/
        TestData.cs
        GardenerTestData.cs
        WorkItemTestData.cs
    Helpers/
    Tests/
    GlobalSetup.cs
```

### Deterministic Test Data

The API should read naturally:

```csharp
TestData.Gardener.Bob
TestData.Gardener.Mary
TestData.Gardener.DeletedDave
TestData.WorkItem.BobMowSmithStreet
TestData.WorkItem.MaryTrimJonesHedge
```

Each assigned WorkItem explicitly references its Gardener, e.g. `Gardener = TestData.Gardener.Bob`; include at least one unassigned WorkItem. Definitions describe logical records, not manual IDs. Seeding assigns deterministic integer IDs and explicitly inserts identities where necessary, so `TestData.Gardener.Bob.Id` is stable without magic IDs scattered through tests.

Include active gardeners, a soft-deleted gardener, WorkItems across gardeners/statuses, assigned/unassigned, scheduled/unscheduled, completed/cancelled, and useful date ranges. Normal Gardener retrieval should exclude the deleted gardener.

After initial setup, deterministic data need not be recreated every run if already present. Tests must tolerate unrelated records from previous runs and avoid mutating shared baseline records unless isolated/restored.

### Search Integration Tests

Focus on `WorkItemService.SearchAsync`: baseline expected WorkItems, one/multiple Gardeners, statuses, scheduled-date ranges, completed/cancelled as appropriate. Exercise the real service/repository/EF/SQL stack.

### Create-and-Search Test

Create a WorkItem with a GUID-derived title, e.g. `Integration Test {Guid.NewGuid()}`. Use shared date/time infrastructure to obtain today's New Zealand business date as `ScheduledDate`. Create through the real stack, search using today's date plus its unique title/appropriate criteria, and assert it returns correctly. Do not reset the database afterwards; reruns must tolerate accumulated rows.

## CSV-Style Expected Result Assertions

For tabular/list integration results, use inline CSV as C# raw strings:

```csharp
const string Expected = """
    Id,Title,Status,Gardener
    1,Mow Smith Street,Scheduled,Bob
    3,Trim Hedge,New,Mary
    """;
```

Create a reusable assertion helper that converts actual collections to the same CSV shape; compares only expected columns; ignores additional properties; optionally requires exact rows or expected-row containment; and outputs copyable actual CSV on failure.

For intentional changes: run test, copy actual CSV over expected, review in Beyond Compare, then accept. Tests should assert what matters without becoming brittle from unrelated columns/rows.

## Unit Tests

Use NUnit and keep this project deliberately small. Add focused `WorkItemValidator` tests: missing Title; missing Address; Scheduled without date; Done without completion date; Cancelled without cancellation date; valid WorkItem.

Validators remain dependency-free where practical. Also include one small Moq test demonstrating genuine dependency mocking. A likely candidate is a service using the shared date/time service so current date/time can be mocked deterministically. If no natural candidate emerges, leave a TODO rather than distort the design.

## Coding Standards and Tooling

Reuse VegiCoop `.editorconfig`, ReSharper settings, naming/formatting, nullable conventions, analyzers, warning levels and relevant shared build settings. Do not introduce competing tooling without need. For React/TypeScript, establish idiomatic equivalents of the intent behind these standards rather than mechanically translating C# conventions.

## Execution Style

**Do not attempt to build the entire solution in one pass.** Work in very small, reviewable increments.

For each increment: make one coherent change; keep the solution compiling/runnable where practical; stop and summarise exactly what changed; identify assumptions/decisions; wait for my next instruction. Do not create speculative code ahead of the current task.

## Initial Backlog

Treat this as a backlog, **not permission to implement everything immediately**:

1. Create solution structure and copy applicable VegiCoop configuration.
2. Create SQL Project and database schema.
3. Add entities and EF context.
4. Add repositories.
5. Add services.
6. Add validators and mappers.
7. Add API controllers and Swagger/OpenAPI.
8. Add integration-test database setup.
9. Add deterministic integration-test data.
10. Add integration tests for search and creation.
11. Add unit tests.
12. Create React/TypeScript/Bootstrap application.
13. Add React API facade.
14. Add WorkItem list UI.
15. Add WorkItem add/edit UI.
16. Add cancellation behaviour.
17. Add filtering/sorting if useful and time permits.
18. Add README/interview notes.

Do not begin the next backlog item until I explicitly ask you to continue.

## React Learning Notes

Create a short `REACT-NOTES.md` alongside the frontend, practical and specific to this solution. Explain where the app starts; component hierarchy; page state and `useState`; API calls/facade; WorkItem rendering; add/edit dialog; forms/validation; app-owned CSS; and where to make common interview changes such as adding a field/filter/column, calling a new API method or changing dialog behaviour.

Where useful, relate React concepts to Blazor/C# without forcing bad analogies. Keep it concise enough for a quick pre-interview refresher.
